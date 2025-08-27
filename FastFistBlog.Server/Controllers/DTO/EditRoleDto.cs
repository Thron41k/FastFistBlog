using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Controllers.DTO;

public class EditRoleDto
{
    public string Id { get; set; } = string.Empty;
    [Required(ErrorMessage = "Название роли не может быть пустым")]
    [Display(Name = "Название роли")]
    [StringLength(256, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Описание роли")]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}