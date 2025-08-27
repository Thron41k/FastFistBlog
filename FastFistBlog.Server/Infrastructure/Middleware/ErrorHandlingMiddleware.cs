using FastFistBlog.Server.Services.Interfaces;

namespace FastFistBlog.Server.Infrastructure.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, IUserActionLogger logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            logger.LogError(userId, "GlobalException", ex, $"Path={context.Request.Path}");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Произошла ошибка на сервере. Обратитесь к администратору."
            });
        }
    }
}