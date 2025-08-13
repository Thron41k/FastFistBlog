using FastFistBlog.Server.Controllers.DTO;
using FastFistBlog.Server.Data;
using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers.Mvc;

[Authorize(Roles = "Administrator")]
public class UsersMvcController(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext context) : Controller
{

    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.ToListAsync();
        var list = new List<UserListItemVm>();
        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            var articles = await context.Articles.CountAsync(a => a.AuthorId == u.Id);
            var comments = await context.Comments.CountAsync(c => c.AuthorId == u.Id);
            list.Add(new UserListItemVm
            {
                Id = u.Id,
                Email = u.Email ?? "",
                UserName = u.UserName!,
                Roles = roles.ToList(),
                ArticlesCount = articles,
                CommentsCount = comments
            });
        }
        return View(list);
    }

    public async Task<IActionResult> Details(string id)
    {
        var u = await userManager.FindByIdAsync(id);
        if (u == null) return NotFound();
        var roles = await userManager.GetRolesAsync(u);
        var articles = await context.Articles.CountAsync(a => a.AuthorId == u.Id);
        var comments = await context.Comments.CountAsync(c => c.AuthorId == u.Id);

        var vm = new UserDetailsVm
        {
            Id = u.Id,
            Email = u.Email ?? "",
            UserName = u.UserName!,
            Roles = roles.ToList(),
            ArticlesCount = articles,
            CommentsCount = comments
        };
        return View(vm);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var u = await userManager.FindByIdAsync(id);
        if (u == null) return NotFound();
        var allRoles = await roleManager.Roles.Select(r => r.Name!).ToListAsync();
        var roles = await userManager.GetRolesAsync(u);
        var vm = new UserEditVm
        {
            Id = u.Id,
            Email = u.Email ?? "",
            UserName = u.UserName!,
            SelectedRoles = roles.ToList(),
            AllRoles = allRoles
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserEditVm model)
    {
        if (id != model.Id) return BadRequest();
        var u = await userManager.FindByIdAsync(id);
        if (u == null) return NotFound();
        u.UserName = model.UserName;
        u.Email = model.Email;
        var updateRes = await userManager.UpdateAsync(u);
        if (!updateRes.Succeeded)
        {
            foreach (var e in updateRes.Errors) ModelState.AddModelError("", e.Description);
        }
        var currentRoles = await userManager.GetRolesAsync(u);
        var toAdd = model.SelectedRoles.Except(currentRoles).ToList();
        var toRemove = currentRoles.Except(model.SelectedRoles).ToList();
        if (toAdd.Count > 0)
        {
            var addRes = await userManager.AddToRolesAsync(u, toAdd);
            if (!addRes.Succeeded)
                foreach (var e in addRes.Errors) ModelState.AddModelError("", e.Description);
        }
        if (toRemove.Count > 0)
        {
            var remRes = await userManager.RemoveFromRolesAsync(u, toRemove);
            if (!remRes.Succeeded)
                foreach (var e in remRes.Errors) ModelState.AddModelError("", e.Description);
        }
        if (ModelState.IsValid) return RedirectToAction(nameof(Index));
        model.AllRoles = await roleManager.Roles.Select(r => r.Name!).ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Delete(string id)
    {
        var u = await userManager.FindByIdAsync(id);
        if (u == null) return NotFound();
        var roles = await userManager.GetRolesAsync(u);
        ViewBag.Roles = roles;
        return View(u);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var u = await userManager.FindByIdAsync(id);
        if (u == null) return NotFound();
        var res = await userManager.DeleteAsync(u);
        if (res.Succeeded)
        {
            TempData["Success"] = "Пользователь удален.";
            return RedirectToAction(nameof(Index));
        }
        foreach (var e in res.Errors) ModelState.AddModelError("", e.Description);
        var roles = await userManager.GetRolesAsync(u);
        ViewBag.Roles = roles;
        return View("Delete", u);
    }

    public async Task<IActionResult> Create()
    {
        var dto = new UserCreateDto
        {
            AllRoles = await roleManager.Roles.Select(r => r.Name!).ToListAsync()
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto model)
    {
        if (!ModelState.IsValid)
        {
            model.AllRoles = await roleManager.Roles.Select(r => r.Name!).ToListAsync();
            return View(model);
        }
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email
        };
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            model.AllRoles = await roleManager.Roles.Select(r => r.Name!).ToListAsync();
            return View(model);
        }
        if (model.SelectedRoles.Count > 0)
        {
            await userManager.AddToRolesAsync(user, model.SelectedRoles);
        }
        TempData["Success"] = "Пользователь успешно создан.";
        return RedirectToAction(nameof(Index));
    }
}
