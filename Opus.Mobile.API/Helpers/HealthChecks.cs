using Opus.Mobile.API.Services.Logging;
using Opus.Mobile.Data.Context;

namespace Opus.Mobile.API.Helpers;

public static class HealthChecks
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    public static async Task<(bool Success, string? Error)> CheckDatabaseAsync(OpusDBContext ctx, ILoggerManager logger)
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(Timeout);

        try
        {
            return await ctx.Database.CanConnectAsync(cts.Token) ?
                (true, null) :
                (false, "CanConnectAsync returned false");
        }
        catch (Exception ex)
        {
            logger.Error($"Db - UNREACHABLE \nReason: {ex.Message}\n");
            return (false, ex.Message);
        }
    }
}
