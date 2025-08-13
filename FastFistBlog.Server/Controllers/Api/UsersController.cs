using System.Security.Claims;
using FastFistBlog.Server.Controllers.DTO;
using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.Server.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
    {
        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.UserName
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new ApplicationRole{Name = "User" });

        await userManager.AddToRoleAsync(user, "User");

        return Ok(new { user.Id, user.Email, user.UserName });
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = userManager.Users.Select(u => new { u.Id, u.Email, u.UserName }).ToList();
        return Ok(users);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        return Ok(new { user.Id, user.Email, user.UserName });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto model)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.UserName = model.UserName;
        user.Email = model.Email;
        user.UserName = model.Email;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { user.Id, user.Email, user.UserName });
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null) return Unauthorized();

        if (!await userManager.CheckPasswordAsync(user, model.Password))
            return Unauthorized();

        var roles = await userManager.GetRolesAsync(user);

        if (user.UserName == null) return BadRequest("Username is empty.");
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Ok("Login successful");
    }

}
