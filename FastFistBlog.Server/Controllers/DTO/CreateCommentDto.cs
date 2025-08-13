using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Controllers.DTO;

public class CreateCommentDto
{
    [Required]
    public int ArticleId { get; set; }
    [Required, StringLength(2000)]
    public string Content { get; set; } = string.Empty;
}