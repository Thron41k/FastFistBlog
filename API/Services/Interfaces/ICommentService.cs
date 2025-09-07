using FastFistBlog.Data.Models;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<Comment>> GetByArticleIdAsync(int articleId);
    Task<Comment?> GetByIdAsync(int id);
    Task<Comment> CreateAsync(CommentDto comment);
    Task<Comment?> UpdateAsync(CommentDto comment);
    Task<bool> DeleteAsync(int id);
}