namespace FastFistBlog.Data.Models.DTO;

public class ArticleDto
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public List<int> Tags { get; set; } = new();
    public List<int> Comments { get; set; } = new();
}