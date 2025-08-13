namespace FastFistBlog.Server.Controllers.DTO;

public class CommentGroupVm
{
    public int ArticleId { get; set; }
    public string ArticleTitle { get; set; } = string.Empty;
    public List<CommentListItemVm> Comments { get; set; } = new();
}