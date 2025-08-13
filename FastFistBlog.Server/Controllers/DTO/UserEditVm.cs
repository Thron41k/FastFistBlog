namespace FastFistBlog.Server.Controllers.DTO;

public class UserEditVm
{
    public string Id { get; set; } = default!;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> SelectedRoles { get; set; } = new();
    public List<string> AllRoles { get; set; } = new();
}