namespace FastFistBlog.Data.Models.DTO;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ArticleId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
}