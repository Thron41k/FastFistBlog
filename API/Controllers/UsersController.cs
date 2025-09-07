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
public class UsersController(IUserService userService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<List<ApplicationUserDto>>> GetAll()
    {
        try
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUserDto>> GetById(string id)
    {
        try
        {
            if (!User.IsInRole("Administrator") && User.FindFirstValue(ClaimTypes.NameIdentifier) != id)
                return Forbid();
            var user = await userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ApplicationUserDto>> Create(ApplicationUserDto dto)
    {
        try
        {
            var user = await userService.CreateAsync(dto);
            var resultDto = mapper.Map<ApplicationUserDto>(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, resultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<ApplicationUserDto>> Update(ApplicationUserDto dto)
    {
        try
        {
            if (!User.IsInRole("Administrator") && User.FindFirstValue(ClaimTypes.NameIdentifier) != dto.UserName)
                return Forbid();
            var user = await userService.UpdateAsync(dto);
            if (user == null) return NotFound();
            var resultDto = mapper.Map<ApplicationUserDto>(user);
            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var deleted = await userService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}