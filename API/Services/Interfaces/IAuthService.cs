using FastFistBlog.Data.Models.DTO;

namespace FastFistBlog.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
}