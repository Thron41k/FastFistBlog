using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(64)]
    public string DisplayName { get; set; } = string.Empty;
    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
