using FastFistBlog.Server.Data;
using FastFistBlog.Server.Data.Models;
using FastFistBlog.Server.Infrastructure.Filters;
using FastFistBlog.Server.Services;
using FastFistBlog.Server.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)
        .EnableSensitiveDataLogging()
        .LogTo(NLog.LogManager.GetCurrentClassLogger().Info, LogLevel.Information));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/AccountMvc/Login";
    options.AccessDeniedPath = "/ErrorMvc/403";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserActionLogger, UserActionLogger>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<UserActionLoggingFilter>();
});
builder.Services.AddRazorPages();

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

await DataSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/ErrorMvc/500");
    app.UseStatusCodePagesWithReExecute("/ErrorMvc/{0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages().WithStaticAssets();

app.Run();
