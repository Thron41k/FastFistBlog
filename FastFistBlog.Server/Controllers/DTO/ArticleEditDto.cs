using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Controllers.DTO;

public class ArticleEditDto
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "Название статьи не может быть пустым")]
    [MinLength(3, ErrorMessage = "Название статьи должно содержать не менее 3 символов")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Контент статьи не может быть пустым")]
    [MinLength(20, ErrorMessage = "Контент статьи должен содержать не менее 20 символов")]
    public string Content { get; set; } = string.Empty;
    public List<TagDto> AllTags { get; set; } = new();
    public List<int> SelectedTagIds { get; set; } = new();
}