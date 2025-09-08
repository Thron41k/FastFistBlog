using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data.Models.DTO;
using FastFistBlog.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.API.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager, IMapper mapper) : IRoleService
{
    public async Task<List<ApplicationRoleDto>> GetAllAsync()
    {
        var roles = await roleManager.Roles.ToListAsync();
        return mapper.Map<List<ApplicationRoleDto>>(roles);
    }

    public async Task<ApplicationRoleDto?> GetByIdAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        return role == null ? null : mapper.Map<ApplicationRoleDto>(role);
    }

    public async Task<ApplicationRole> CreateAsync(ApplicationRoleDto dto)
    {
        var role = mapper.Map<ApplicationRole>(dto);
        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return role;
    }

    public async Task<ApplicationRole?> UpdateAsync(string id, ApplicationRoleDto dto)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return null;

        role.Name = dto.Name;
        role.NormalizedName = dto.Name.ToUpper();
        role.Description = dto.Description;

        var result = await roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return role;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var role = await roleManager.FindByIdAsync(id);
        if (role == null) return false;

        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded;
    }
}