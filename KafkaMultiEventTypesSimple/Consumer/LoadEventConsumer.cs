using Domain;
using MassTransit;

namespace Consumer;

public class LoadEventConsumer : IConsumer<ILoadEvent>, IConsumer<LoadAddedEvent>
{
    private readonly ILogger<LoadAddedEvent> _logger;

    public LoadEventConsumer(ILogger<LoadAddedEvent> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ILoadEvent> context)
    {
        _logger.LogInformation("Consume {LoadEvent}", context.Message);

        await Task.CompletedTask;
    }

    public async Task Consume(ConsumeContext<LoadAddedEvent> context)
    {
        _logger.LogInformation("Consume {LoadAddedEvent}", context.Message);

        await Task.CompletedTask;
    }
}