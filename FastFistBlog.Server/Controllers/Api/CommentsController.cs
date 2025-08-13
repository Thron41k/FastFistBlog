using FastFistBlog.Server.Controllers.DTO;
using FastFistBlog.Server.Data;
using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : ControllerBase
{
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var comments = await context.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedAt,
                Author = c.Author!.DisplayName,
                c.ArticleId
            })
            .ToListAsync();

        return Ok(comments);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var comment = await context.Comments
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null) return NotFound();

        return Ok(new
        {
            comment.Id,
            comment.Content,
            comment.CreatedAt,
            Author = comment.Author!.DisplayName
        });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var article = await context.Articles.FindAsync(model.ArticleId);
        if (article == null) return NotFound("Article not found.");

        var comment = new Comment
        {
            Content = model.Content,
            ArticleId = model.ArticleId,
            AuthorId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        return Ok(new { comment.Id, comment.Content });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentDto model)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var user = await userManager.GetUserAsync(User);
        if (user == null || comment.AuthorId != user.Id)
            return Forbid();

        comment.Content = model.Content;

        context.Comments.Update(comment);
        await context.SaveChangesAsync();

        return Ok(new { comment.Id, comment.Content });
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var user = await userManager.GetUserAsync(User);
        if (user == null || comment.AuthorId != user.Id)
            return Forbid();

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();

        return Ok();
    }
}