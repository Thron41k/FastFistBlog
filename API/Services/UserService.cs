using AutoMapper;
using FastFistBlog.API.Services.Interfaces;
using FastFistBlog.Data;
using FastFistBlog.Data.Models;
using FastFistBlog.Data.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFistBlog.API.Services;

public class UserService(ApplicationDbContext db, IMapper mapper, UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync()
    {
        var users = await db.Users
            .Include(u => u.Articles)
            .Include(u => u.Comments)
            .ToListAsync();
        return mapper.Map<IEnumerable<ApplicationUserDto>>(users);
    }

    public async Task<ApplicationUserDto?> GetUserByIdAsync(string userId)
    {
        var user = await db.Users
            .Include(u => u.Articles)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return null;
        return mapper.Map<ApplicationUserDto>(user);
    }

    public async Task<ApplicationUser> CreateAsync(ApplicationUserDto dto)
    {
        var user = mapper.Map<ApplicationUser>(dto);
        user.Id = Guid.NewGuid().ToString();
        user.PasswordHash = dto.PasswordHash;
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return user;
    }

    public async Task<ApplicationUser?> UpdateAsync(ApplicationUserDto dto)
    {
        var user = await db.Users
            .Include(u => u.Articles)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.Id == dto.UserName);
        if (user == null) return null;
        user.DisplayName = dto.DisplayName;
        user.Email = dto.Email;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await db.Users.FindAsync(id);
        if (user == null) return false;
        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}