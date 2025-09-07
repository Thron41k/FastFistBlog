using Microsoft.AspNetCore.Identity;

namespace FastFistBlog.Data.Models;

public class ApplicationRole : IdentityRole
{
    public string Description { get; set; } = string.Empty;
}