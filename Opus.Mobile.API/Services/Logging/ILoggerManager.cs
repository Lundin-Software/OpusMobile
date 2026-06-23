namespace Opus.Mobile.API.Services.Logging;

public interface ILoggerManager
{
    void Info(string message);

    void Warn(string message);

    void Debug(string message);

    void Error(string message);

    void Error(string message, Exception ex);
}
