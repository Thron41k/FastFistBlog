using FastFistBlog.Data;
using FastFistBlog.Data.Models;
using FastFistBlog.Server.Controllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers.Mvc;

public class CommentsMvcController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : Controller
{
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Index()
    {
        var groups = await context.Comments
            .Include(c => c.Article)
            .Include(c => c.Author)
            .OrderBy(c => c.ArticleId)
            .ThenByDescending(c => c.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
        var vm = groups
            .GroupBy(c => new { c.ArticleId, ArticleTitle = c.Article!.Title })
            .Select(g => new CommentGroupVm
            {
                ArticleId = g.Key.ArticleId,
                ArticleTitle = g.Key.ArticleTitle,
                Comments = g.Select(c => new CommentListItemVm
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    AuthorDisplayName = (c.Author!.UserName ?? c.Author.Email) ?? string.Empty
                }).ToList()
            })
            .OrderBy(g => g.ArticleTitle)
            .ToList();
        return View(vm);
    }

    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Details(int id)
    {
        var comment = await context.Comments
            .Include(x => x.Article)
            .Include(x => x.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return NotFound();
        var vm = new CommentEditVm
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            ArticleTitle = comment.Article!.Title,
            AuthorDisplayName = comment.Author!.UserName ?? comment.Author.Email
        };
        return View(vm);
    }

    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Edit(int id)
    {
        var comment = await context.Comments
            .Include(x => x.Article)
            .Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return NotFound();
        if (!CanEditOrDelete(comment))
            return Forbid();
        var vm = new CommentEditVm
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            ArticleTitle = comment.Article!.Title,
            AuthorDisplayName = (comment.Author!.UserName ?? comment.Author.Email) ?? string.Empty
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Edit(CommentEditVm model)
    {
        if (!ModelState.IsValid) return View(model);
        var comment = await context.Comments
            .Include(x => x.Article)
            .Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == model.Id);
        if (comment == null) return NotFound();
        if (!CanEditOrDelete(comment))
            return Forbid();
        comment.Content = model.Content;
        await context.SaveChangesAsync();
        TempData["Success"] = "Комментарий обновлен.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await context.Comments
            .Include(x => x.Article)
            .Include(x => x.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return NotFound();
        if (!CanEditOrDelete(comment))
            return Forbid();
        var vm = new CommentEditVm
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            ArticleTitle = comment.Article!.Title,
            AuthorDisplayName = (comment.Author!.UserName ?? comment.Author.Email) ?? string.Empty
        };
        return View(vm);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) return NotFound();
        if (!CanEditOrDelete(comment))
            return Forbid();
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
        TempData["Success"] = "Комментарий удален.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CreateCommentDto model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("Details", "ArticlesMvc", new
            {
                id = model.ArticleId
            });
        var article = await context.Articles.FindAsync(model.ArticleId);
        if (article == null)
            return NotFound();
        var userId = userManager.GetUserId(User);
        var comment = new Comment
        {
            Content = model.Content,
            ArticleId = model.ArticleId,
            AuthorId = userId ?? "",
            CreatedAt = DateTime.UtcNow
        };
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return RedirectToAction("Details", "ArticlesMvc", new
        {
            id = model.ArticleId
        });
    }

    private bool CanEditOrDelete(Comment comment)
    {
        var userName = User.Identity?.Name;
        return User.IsInRole("Administrator") ||
               User.IsInRole("Moderator") ||
               comment.Author is { } user && user.UserName == userName;
    }
}
