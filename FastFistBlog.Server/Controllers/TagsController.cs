using FastFistBlog.Server.Controllers.DTO;
using FastFistBlog.Server.Data;
using FastFistBlog.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(ApplicationDbContext context) : ControllerBase
{
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await context.Tags
            .Select(t => new { t.Id, t.Name })
            .ToListAsync();

        return Ok(tags);
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        return Ok(new { tag.Id, tag.Name });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagDto model)
    {
        if (await context.Tags.AnyAsync(t => t.Name == model.Name))
            return BadRequest("Tag with the same name already exists.");

        var tag = new Tag
        {
            Name = model.Name.Trim()
        };

        context.Tags.Add(tag);
        await context.SaveChangesAsync();

        return Ok(new { tag.Id, tag.Name });
    }

    [Authorize(Roles = "Administrator")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagDto model)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        tag.Name = model.Name.Trim();

        context.Tags.Update(tag);
        await context.SaveChangesAsync();

        return Ok(new { tag.Id, tag.Name });
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null) return NotFound();

        context.Tags.Remove(tag);
        await context.SaveChangesAsync();

        return Ok();
    }
}
