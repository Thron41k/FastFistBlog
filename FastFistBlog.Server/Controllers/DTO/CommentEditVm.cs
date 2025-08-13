namespace FastFistBlog.Server.Controllers.DTO;

public class CommentEditVm
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ArticleTitle { get; set; } = string.Empty;
    public string? AuthorDisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}