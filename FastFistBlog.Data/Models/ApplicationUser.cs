using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FastFistBlog.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(64)]
    public string DisplayName { get; set; } = string.Empty;
    public ICollection<Article> Articles { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}
