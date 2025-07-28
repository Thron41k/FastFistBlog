using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Data.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}