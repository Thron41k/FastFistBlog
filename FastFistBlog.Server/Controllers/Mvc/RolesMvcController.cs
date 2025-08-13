using FastFistBlog.Server.Controllers.DTO;
using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.Server.Controllers.Mvc;

[Authorize(Roles = "Administrator")]
public class RolesMvcController(RoleManager<ApplicationRole> roleManager) : Controller
{
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateRoleDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleDto model)
    {
        if (!ModelState.IsValid)
            return View(model);
        if (await roleManager.RoleExistsAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), "Роль с таким именем уже существует.");
            return View(model);
        }
        var role = new ApplicationRole
        {
            Name = model.Name.Trim(),
            Description = model.Description.Trim()
        };
        var result = await roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }
        TempData["Success"] = "Роль успешно создана.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Index()
    {
        var roles = roleManager.Roles
            .Select(r => new RoleListItemDto
            {
                Id = r.Id,
                Name = r.Name!,
                Description = r.Description
            })
            .ToList();
        return View(roles);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        var model = new EditRoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditRoleDto model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var role = await roleManager.FindByIdAsync(model.Id);
        if (role == null) return NotFound();
        role.Name = model.Name.Trim();
        role.Description = model.Description.Trim();
        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);
            return View(model);
        }
        TempData["Success"] = "Роль обновлена.";
        return RedirectToAction(nameof(Index));
    }


    public async Task<IActionResult> Delete(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        return View(role);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        await roleManager.DeleteAsync(role);
        TempData["Success"] = "Роль удалена.";
        return RedirectToAction(nameof(Index));
    }

}