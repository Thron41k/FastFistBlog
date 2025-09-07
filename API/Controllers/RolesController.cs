using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFistBlog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class RolesController(IRoleService roleService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ApplicationRoleDto>>> GetAll()
    {
        try
        {
            var roles = await roleService.GetAllAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationRoleDto>> GetById(string id)
    {
        try
        {
            var role = await roleService.GetByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationRoleDto>> Create(ApplicationRoleDto dto)
    {
        try
        {
            var role = await roleService.CreateAsync(dto);
            var resultDto = mapper.Map<ApplicationRoleDto>(role);

            return CreatedAtAction(nameof(GetById), new { id = role.Id }, resultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationRoleDto>> Update(string id, ApplicationRoleDto dto)
    {
        try
        {
            var role = await roleService.UpdateAsync(id, dto);
            if (role == null) return NotFound();

            var resultDto = mapper.Map<ApplicationRoleDto>(role);
            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        try
        {
            var deleted = await roleService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}