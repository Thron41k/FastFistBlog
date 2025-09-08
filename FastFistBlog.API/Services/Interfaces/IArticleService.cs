using FastFistBlog.Data.Models;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services.Interfaces;

public interface IArticleService
{
    Task<IEnumerable<Article>> GetAllAsync();
    Task<Article?> GetByIdAsync(int id);
    Task<Article> CreateAsync(ArticleDto articleDto);
    Task<Article?> UpdateAsync(ArticleDto articleDto);
    Task<bool> DeleteAsync(int id);
}