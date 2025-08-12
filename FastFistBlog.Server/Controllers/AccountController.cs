using FastFistBlog.Server.Controllers.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace FastFistBlog.Server.Controllers
{
    public class AccountController(IHttpClientFactory httpClientFactory) : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

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
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}");
            var response = await client.PostAsJsonAsync("/api/users/login", model);

            if (response.IsSuccessStatusCode)
            {
                return Redirect(returnUrl ?? Url.Action("Index", "Home")!);
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
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

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}");

            var response = await client.PostAsJsonAsync("/api/users/register", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            var errors = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Ошибка регистрации: {errors}");
            return View(model);
        }
    }
}
