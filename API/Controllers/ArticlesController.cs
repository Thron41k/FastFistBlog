using System.Security.Claims;
using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ArticlesController(IArticleService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleDto>>> GetAll()
    {
        try
        {
            var articles = await service.GetAllAsync();
            var dtos = mapper.Map<IEnumerable<ArticleDto>>(articles);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ArticleDto>> GetById(int id)
    {
        try
        {
            var article = await service.GetByIdAsync(id);
            if (article is null) return NotFound();
            var dto = mapper.Map<ArticleDto>(article);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ArticleDto>> Create(ArticleDto articleDto)
    {
        try
        {
            var created = await service.CreateAsync(articleDto);
            var dto = mapper.Map<ArticleDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ArticleDto articleDto)
    {
        try
        {
            if ((!User.IsInRole("Moderator") || !User.IsInRole("Administrator")) &&
            User.FindFirstValue(ClaimTypes.NameIdentifier) != articleDto.AuthorId)
                return Forbid();
            if (id != articleDto.Id) return BadRequest();
            var updated = await service.UpdateAsync(articleDto);
            var dto = mapper.Map<ArticleDto>(updated);
            return updated is null ? NotFound() : Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var article = await service.GetByIdAsync(id);
            if (article != null)
            {
                if ((!User.IsInRole("Moderator") || !User.IsInRole("Administrator")) &&
                    User.FindFirstValue(ClaimTypes.NameIdentifier) != article.AuthorId)
                    return Forbid();
            }
            else
            {
                return NotFound();
            }
            var deleted = await service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}