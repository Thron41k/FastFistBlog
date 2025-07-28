using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Data.Models;

public class Comment
{
    [Key]
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ArticleId { get; set; }
    public Article Article { get; set; }
    public string AuthorId { get; set; }
    public ApplicationUser Author { get; set; }
}
