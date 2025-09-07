namespace FastFistBlog.Data.Models.DTO;

public class ApplicationUserDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<int> Articles { get; set; } = new();
    public List<int> Comments { get; set; } = new();
}