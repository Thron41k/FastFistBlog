using FastFistBlog.Data.Models;
using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync();
    Task<ApplicationUserDto?> GetUserByIdAsync(string userId);
    Task<ApplicationUser> CreateAsync(ApplicationUserDto applicationUserDto);
    Task<ApplicationUser?> UpdateAsync(ApplicationUserDto applicationUserDto);
    Task<bool> DeleteAsync(string userId);
}