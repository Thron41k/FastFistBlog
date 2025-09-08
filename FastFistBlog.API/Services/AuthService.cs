using System.IdentityModel.Tokens.Jwt;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace FastFistBlog.API.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
    : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user == null) return null;

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid) return null;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new("DisplayName", user.DisplayName ?? "")
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expiration
        };
    }
}