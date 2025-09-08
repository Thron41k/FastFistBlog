using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController(ITagService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        try
        {
            var tags = await service.GetAllAsync();
            var dtos = mapper.Map<IEnumerable<TagDto>>(tags);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TagDto>> GetById(int id)
    {
        try
        {
            var tag = await service.GetByIdAsync(id);
            if (tag is null) return NotFound();
            var dto = mapper.Map<TagDto>(tag);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<ActionResult<TagDto>> Create(TagDto tagDto)
    {
        try
        {
            var created = await service.CreateAsync(tagDto);
            var dto = mapper.Map<TagDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Update(int id, TagDto tagDto)
    {
        try
        {
            if (id != tagDto.Id) return BadRequest();
            var updated = await service.UpdateAsync(tagDto);
            var dto = mapper.Map<TagDto>(updated);
            return updated is null ? NotFound() : Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator,Moderator")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}