using FastFistBlog.Data.Models;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services.Interfaces;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<Tag> CreateAsync(TagDto tagDto);
    Task<Tag?> UpdateAsync(TagDto tagDto);
    Task<bool> DeleteAsync(int id);
}