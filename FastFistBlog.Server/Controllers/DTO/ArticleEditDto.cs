namespace FastFistBlog.Server.Controllers.DTO;

public class ArticleEditDto
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<TagDto> AllTags { get; set; } = new();
    public List<int> SelectedTagIds { get; set; } = new();
}