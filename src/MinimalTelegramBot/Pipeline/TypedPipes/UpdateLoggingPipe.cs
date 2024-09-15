using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalTelegramBot.Pipeline.TypedPipes;

internal sealed class UpdateLoggingPipe : IPipe
{
    private readonly ILogger<UpdateLoggingPipe> _logger;

    public UpdateLoggingPipe(ILogger<UpdateLoggingPipe> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(BotRequestContext ctx, BotRequestDelegate next)
    {
        _logger.LogInformation(0, "Received update with ID = {UpdateId}", ctx.Update.Id);

        var stopWatch = Stopwatch.StartNew();

        await next(ctx);

        stopWatch.Stop();

        if (ctx.Data.ContainsKey("__UpdateHandlingStarted"))
        {
            _logger.LogInformation(200, "Update with ID = {UpdateId} is handled. Duration: {Milliseconds} ms", ctx.Update.Id, stopWatch.ElapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation(404, "Update with ID = {UpdateId} is not handled", ctx.Update.Id);
        }
    }
}