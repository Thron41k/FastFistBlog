using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Data.Models;

public class Article
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required, StringLength(2000)]
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required, StringLength(32)]
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser? Author { get; set; }
    public ICollection<ArticleTag> ArticleTags { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}