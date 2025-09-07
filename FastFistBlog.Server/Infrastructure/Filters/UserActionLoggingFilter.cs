using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using FastFistBlog.Server.Services.Interfaces;

namespace FastFistBlog.Server.Infrastructure.Filters;

public class UserActionLoggingFilter(IUserActionLogger actionLogger) : IActionFilter, IExceptionFilter
{
    private string GetUserId(FilterContext context) =>
        context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = GetUserId(context);
        var actionName = context.ActionDescriptor.DisplayName;
        var arguments = string.Join(", ", context.ActionArguments.Select(a => $"{a.Key}={a.Value}"));

        actionLogger.LogInfo(userId, "ActionExecuting", $"Action={actionName}; Args={arguments}");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var userId = GetUserId(context);
        var actionName = context.ActionDescriptor.DisplayName;

        if (context.Exception == null)
        {
            actionLogger.LogInfo(userId, "ActionExecuted", $"Action={actionName}; Result={context.Result}");
        }
    }

    public void OnException(ExceptionContext context)
    {
        var userId = GetUserId(context);
        var actionName = context.ActionDescriptor.DisplayName;
        var exception = context.Exception;

        actionLogger.LogError(userId, "ActionException", exception, $"Action={actionName}");
    }
}