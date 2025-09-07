using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Controllers.DTO;

public class CreateRoleDto
{
    [Required(ErrorMessage = "Название роли не может быть пустым")]
    [Display(Name = "Название роли")]
    [StringLength(256, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [Display(Name = "Описание роли")]
    [DataType(DataType.MultilineText)]
    [StringLength(2000, ErrorMessage = "Максимальная длина описания — 2000 символов.")]
    public string Description { get; set; } = string.Empty;
}