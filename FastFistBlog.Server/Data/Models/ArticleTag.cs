using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Data.Models;

public class ArticleTag
{
    [Key]
    public int ArticleId { get; set; }
    public Article? Article { get; set; }
    public int TagId { get; set; }
    public Tag? Tag { get; set; }
}