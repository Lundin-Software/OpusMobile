using NLog;

namespace Opus.Mobile.API.Services.Logging;

public class LoggerManager : ILoggerManager
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public void Debug(string message) => logger.Debug(message);

    public void Info(string message) => logger.Info(message);

    public void Warn(string message) => logger.Warn(message);

    public void Error(string message) => logger.Error(message);

    public void Error(string message, Exception ex) => logger.Error(ex, message);
}
