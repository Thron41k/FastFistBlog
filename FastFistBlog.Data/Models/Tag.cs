using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Data.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    [Required, StringLength(64)]
    public string Name { get; set; } = string.Empty;
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}