using FastFistBlog.Server.Controllers.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FastFistBlog.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace FastFistBlog.Server.Controllers.Mvc;

public class AccountMvcController(
    IHttpClientFactory httpClientFactory,
    UserManager<ApplicationUser> userManager) : Controller
{

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
        {
            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            new ClaimsPrincipal(claimsIdentity));
        return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterUserDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserDto model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}");
        var response = await client.PostAsJsonAsync("/api/users/register", model);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Login");
        }
        var errorsJson = await response.Content.ReadAsStringAsync();
        var identityErrors = System.Text.Json.JsonSerializer.Deserialize<List<IdentityErrorDto>>(
            errorsJson,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );
        if (identityErrors != null)
        {
            foreach (var error in identityErrors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Произошла ошибка регистрации.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
    }
}