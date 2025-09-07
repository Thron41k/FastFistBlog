using FastFistBlog.Data;
using FastFistBlog.Data.Models;
using FastFistBlog.Server.Controllers.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers.Mvc
{
    [Authorize(Roles = "Administrator,Moderator")]
    public class TagsMvcController(ApplicationDbContext context) : Controller
    {

        [HttpGet("/Tags")]
        public async Task<IActionResult> Index()
        {
            var tags = await context.Tags.ToListAsync();
            return View(tags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTagDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (await context.Tags.AnyAsync(t => t.Name == model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Такой тег уже существует.");
                return View(model);
            }
            var tag = new Tag { Name = model.Name.Trim() };
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
            TempData["Success"] = "Тег создан.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null) return NotFound();
            var model = new UpdateTagDto { Name = tag.Name };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTagDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var tag = await context.Tags.FindAsync(id);
            if (tag == null) return NotFound();
            if (await context.Tags.AnyAsync(t => t.Name == model.Name && t.Id != id))
            {
                ModelState.AddModelError(nameof(model.Name), "Такой тег уже существует.");
                return View(model);
            }
            tag.Name = model.Name.Trim();
            context.Tags.Update(tag);
            await context.SaveChangesAsync();
            TempData["Success"] = "Тег изменен.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null) return NotFound();
            return View(tag);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null) return NotFound();
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            TempData["Success"] = "Тег удален.";
            return RedirectToAction(nameof(Index));
        }
    }
}
