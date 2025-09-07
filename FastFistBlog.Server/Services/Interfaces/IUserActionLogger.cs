namespace FastFistBlog.Server.Services.Interfaces;

public interface IUserActionLogger
{
    void LogInfo(string userId, string action, string? details = null);
    void LogWarning(string userId, string action, string? details = null);
    void LogError(string userId, string action, Exception ex, string? details = null);
}