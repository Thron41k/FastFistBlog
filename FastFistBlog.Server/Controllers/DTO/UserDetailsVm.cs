namespace FastFistBlog.Server.Controllers.DTO;

public class UserDetailsVm
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int ArticlesCount { get; set; }
    public int CommentsCount { get; set; }
}