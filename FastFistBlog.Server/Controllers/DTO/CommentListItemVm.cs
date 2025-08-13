namespace FastFistBlog.Server.Controllers.DTO;

public class CommentListItemVm
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string AuthorDisplayName { get; set; } = string.Empty;
}