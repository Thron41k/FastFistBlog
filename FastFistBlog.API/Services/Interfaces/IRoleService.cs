using FastFistBlog.Data.Models.DTO;
using FastFistBlog.Data.Models;

namespace FastFistBlog.API.Services.Interfaces;

public interface IRoleService
{
    Task<List<ApplicationRoleDto>> GetAllAsync();
    Task<ApplicationRoleDto?> GetByIdAsync(string id);
    Task<ApplicationRole> CreateAsync(ApplicationRoleDto dto);
    Task<ApplicationRole?> UpdateAsync(string id, ApplicationRoleDto dto);
    Task<bool> DeleteAsync(string id);
}