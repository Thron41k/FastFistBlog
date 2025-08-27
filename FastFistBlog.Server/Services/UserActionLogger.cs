using FastFistBlog.Server.Services.Interfaces;
using NLog;
using ILogger = NLog.ILogger;

namespace FastFistBlog.Server.Services;

public class UserActionLogger : IUserActionLogger
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public void LogInfo(string userId, string action, string? details = null)
    {
        Logger.Info($"UserId={userId} | Action={action} | {details}");
    }

    public void LogWarning(string userId, string action, string? details = null)
    {
        Logger.Warn($"UserId={userId} | Action={action} | {details}");
    }

    public void LogError(string userId, string action, Exception ex, string? details = null)
    {
        Logger.Error(ex, $"UserId={userId} | Action={action} | {details}");
    }
}