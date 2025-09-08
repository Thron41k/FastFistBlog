using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models;
using FastFistBlog.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services;

public class TagService(ApplicationDbContext db, IMapper mapper) : ITagService
{
    public async Task<IEnumerable<Tag>> GetAllAsync() =>
        await db.Tags
            .Include(t => t.ArticleTags)
            .ThenInclude(at => at.Article)
            .ToListAsync();

    public async Task<Tag?> GetByIdAsync(int id) =>
        await db.Tags
            .Include(t => t.ArticleTags)
            .ThenInclude(at => at.Article)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Tag> CreateAsync(TagDto tagDto)
    {
        var tag = mapper.Map<Tag>(tagDto);
        db.Tags.Add(tag);
        await db.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag?> UpdateAsync(TagDto tagDto)
    {
        if (!db.Tags.Any(t => t.Id == tagDto.Id))
            return null;
        var tag = mapper.Map<Tag>(tagDto);
        db.Tags.Update(tag);
        await db.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await db.Tags.FindAsync(id);
        if (entity is null) return false;
        db.Tags.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}