using System.ComponentModel.DataAnnotations;

namespace FastFistBlog.Server.Controllers.DTO;

public class CreateTagDto
{
    [Required(ErrorMessage = "Название тега обязательно")]
    public string Name { get; set; } = string.Empty;
}