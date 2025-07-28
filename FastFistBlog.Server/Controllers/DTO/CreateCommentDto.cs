namespace FastFistBlog.Server.Controllers.DTO;

public class CreateCommentDto
{
    public int ArticleId { get; set; }
    public string Content { get; set; } = string.Empty;
}