using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models;
using FastFistBlog.Data;
using Microsoft.EntityFrameworkCore;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services;

public class ArticleService(ApplicationDbContext db, IMapper mapper) : IArticleService
{
    public async Task<IEnumerable<Article>> GetAllAsync() =>
        await db.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .ToListAsync();

    public async Task<Article?> GetByIdAsync(int id) =>
        await db.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Article> CreateAsync(ArticleDto articleDto)
    {
        var article = mapper.Map<Article>(articleDto);
        db.Articles.Add(article);
        await db.SaveChangesAsync();
        return article;
    }

    public async Task<Article?> UpdateAsync(ArticleDto articleDto)
    {
        if (!db.Articles.Any(a => a.Id == articleDto.Id))
            return null;
        var article = mapper.Map<Article>(articleDto);
        db.Articles.Update(article);
        await db.SaveChangesAsync();
        return article;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Articles.FindAsync(id);
        if (entity is null) return false;
        db.Articles.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}