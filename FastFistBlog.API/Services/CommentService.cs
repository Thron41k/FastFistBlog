using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models;
using FastFistBlog.Data;
using Microsoft.EntityFrameworkCore;
using FastFistBlog.Data.Models.DTO;
using AutoMapper;

namespace FastFistBlog.API.Services;

public class CommentService(ApplicationDbContext db, IMapper mapper) : ICommentService
{
    public async Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId) =>
        await db.Comments
            .Where(c => c.ArticleId == articleId)
            .Include(c => c.Author)
            .ToListAsync();

    public async Task<Comment?> GetByIdAsync(int id) =>
        await db.Comments
            .Include(c => c.Author)
            .Include(c => c.Article)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Comment> CreateAsync(CommentDto commentDto)
    {
        var comment = mapper.Map<Comment>(commentDto);
        db.Comments.Add(comment);
        await db.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> UpdateAsync(CommentDto commentDto)
    {
        if (!db.Comments.Any(c => c.Id == commentDto.Id))
            return null;
        var comment = mapper.Map<Comment>(commentDto);
        db.Comments.Update(comment);
        await db.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Comments.FindAsync(id);
        if (entity is null) return false;
        db.Comments.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}