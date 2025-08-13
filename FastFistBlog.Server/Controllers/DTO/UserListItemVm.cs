namespace FastFistBlog.Server.Controllers.DTO;

public class UserListItemVm
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int ArticlesCount { get; set; }
    public int CommentsCount { get; set; }
}