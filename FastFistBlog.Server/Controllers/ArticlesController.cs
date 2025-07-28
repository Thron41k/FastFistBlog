using FastFistBlog.Server.Controllers.DTO;
using FastFistBlog.Server.Data;
using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    : ControllerBase
{
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var articles = await context.Articles
            .Include(a => a.Author)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.CreatedAt,
                Author = a.Author.DisplayName
            })
            .ToListAsync();

        return Ok(articles);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("by-author/{authorId}")]
    public async Task<IActionResult> GetByAuthor(string authorId)
    {
        var articles = await context.Articles
            .Where(a => a.AuthorId == authorId)
            .Include(a => a.Author)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.CreatedAt,
                Author = a.Author.DisplayName
            })
            .ToListAsync();

        return Ok(articles);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleDto model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var article = new Article
        {
            Title = model.Title,
            Content = model.Content,
            AuthorId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        context.Articles.Add(article);
        await context.SaveChangesAsync();

        return Ok(new { article.Id, article.Title });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDto model)
    {
        var article = await context.Articles.FindAsync(id);
        if (article == null) return NotFound();

        var user = await userManager.GetUserAsync(User);
        if (user == null || article.AuthorId != user.Id)
            return Forbid();

        article.Title = model.Title;
        article.Content = model.Content;

        context.Articles.Update(article);
        await context.SaveChangesAsync();

        return Ok(new { article.Id, article.Title });
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var article = await context.Articles.FindAsync(id);
        if (article == null) return NotFound();

        var user = await userManager.GetUserAsync(User);
        if (user == null || article.AuthorId != user.Id)
            return Forbid();

        context.Articles.Remove(article);
        await context.SaveChangesAsync();

        return Ok();
    }
}

