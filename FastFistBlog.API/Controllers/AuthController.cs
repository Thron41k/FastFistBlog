using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var response = await authService.LoginAsync(loginDto);
        if (response == null) return Unauthorized("Invalid username or password");
        return Ok(response);
    }
}