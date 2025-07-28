using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Data.Models;

public class Article
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string AuthorId { get; set; }
    public ApplicationUser Author { get; set; }
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}