using FastFistBlog.Data;
using FastFistBlog.Data.Models;
using FastFistBlog.Server.Controllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers.Mvc;

[Authorize]
public class ArticlesMvcController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : Controller
{
    private async Task<ArticleEditDto> PrepareArticleEditDto(Article? article = null)
    {
        var allTags = await context.Tags
            .OrderBy(t => t.Name)
            .Select(t => new TagDto { Id = t.Id, Name = t.Name })
            .ToListAsync();
        return new ArticleEditDto
        {
            Id = article?.Id,
            Title = article?.Title ?? string.Empty,
            Content = article?.Content ?? string.Empty,
            AllTags = allTags,
            SelectedTagIds = article?.ArticleTags.Select(at => at.TagId).ToList() ?? []
        };
    }

    public async Task<IActionResult> Index()
    {
        var articles = await context.Articles
            .Include(a => a.Author)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
        return View(articles);
    }

    public async Task<IActionResult> Details(int id)
    {
        var article = await context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
            .ThenInclude(at => at.Tag)
            .Include(a => a.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article == null) return NotFound();
        return View(article);
    }

    public async Task<IActionResult> Create()
    {
        var dto = await PrepareArticleEditDto();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArticleEditDto model)
    {
        if (!ModelState.IsValid)
        {
            model.AllTags = await context.Tags
                .OrderBy(t => t.Name)
                .Select(t => new TagDto { Id = t.Id, Name = t.Name })
                .ToListAsync();

            return View(model);
        }
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Forbid();

        var article = new Article
        {
            Title = model.Title,
            Content = model.Content,
            AuthorId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ArticleTags = model.SelectedTagIds.Select(tagId => new ArticleTag { TagId = tagId }).ToList()
        };
        context.Articles.Add(article);
        await context.SaveChangesAsync();
        TempData["Success"] = "Статья успешно создана.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var article = await context.Articles
            .Include(a => a.ArticleTags)
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article == null)
            return NotFound();
        if (!CanEditOrDelete(article))
            return Forbid();
        var dto = await PrepareArticleEditDto(article);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArticleEditDto model)
    {
        if (id != model.Id)
            return BadRequest();
        if (!ModelState.IsValid)
        {
            model.AllTags = await context.Tags
                .OrderBy(t => t.Name)
                .Select(t => new TagDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
            return View(model);
        }
        var article = await context.Articles
            .Include(a => a.ArticleTags)
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article == null)
            return NotFound();
        if (!CanEditOrDelete(article))
            return Forbid();
        article.Title = model.Title;
        article.Content = model.Content;
        context.ArticleTags.RemoveRange(article.ArticleTags);
        article.ArticleTags = model.SelectedTagIds.Select(tagId => new ArticleTag
        {
            ArticleId = id,
            TagId = tagId
        }).ToList();
        await context.SaveChangesAsync();
        TempData["Success"] = "Статья обновлена.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var article = await context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article == null) return NotFound();
        if (!CanEditOrDelete(article))
            return Forbid();
        return View(article);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var article = await context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (article == null) return NotFound();
        if (!CanEditOrDelete(article))
            return Forbid();
        context.Articles.Remove(article);
        await context.SaveChangesAsync();
        TempData["Success"] = "Статья удален.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(CreateCommentDto model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Details), new
            {
                id = model.ArticleId
            });
        var article = await context.Articles.FindAsync(model.ArticleId);
        if (article == null) return NotFound();
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Forbid();
        var comment = new Comment
        {
            ArticleId = model.ArticleId,
            Content = model.Content,
            AuthorId = user.Id,
            CreatedAt = DateTime.UtcNow
        };
        context.Comments.Add(comment);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new
        {
            id = model.ArticleId
        });
    }

    private bool CanEditOrDelete(Article article)
    {
        var userName = User.Identity?.Name;
        return User.IsInRole("Administrator") ||
               User.IsInRole("Moderator") ||
              article.Author is { } user && user.UserName == userName;
    }
}
