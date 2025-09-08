using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FastFistBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController(ICommentService service, IMapper mapper) : ControllerBase
{
    // Получить комментарии к статье
    [HttpGet("article/{articleId:int}")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetByArticle(int articleId)
    {
        try
        {
            var comments = await service.GetByArticleIdAsync(articleId);
            var dtos = mapper.Map<IEnumerable<CommentDto>>(comments);
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetById(int id)
    {
        try
        {
            var comment = await service.GetByIdAsync(id);
            if (comment is null) return NotFound();
            var dto = mapper.Map<CommentDto>(comment);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(CommentDto commentDto)
    {
        try
        {
            var created = await service.CreateAsync(commentDto);
            var dto = mapper.Map<CommentDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CommentDto commentDto)
    {
        try
        {
            if ((!User.IsInRole("Moderator") || !User.IsInRole("Administrator")) &&
                User.FindFirstValue(ClaimTypes.NameIdentifier) != commentDto.AuthorId)
                return Forbid();
            if (id != commentDto.Id) return BadRequest();
            var updated = await service.UpdateAsync(commentDto);
            var dto = mapper.Map<CommentDto>(updated);
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
            var comment = await service.GetByIdAsync(id);
            if (comment != null)
            {
                if ((!User.IsInRole("Moderator") || !User.IsInRole("Administrator")) &&
                    User.FindFirstValue(ClaimTypes.NameIdentifier) != comment.AuthorId)
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