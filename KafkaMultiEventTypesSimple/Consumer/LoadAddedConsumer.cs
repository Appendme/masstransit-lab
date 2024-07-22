using Domain;
using MassTransit;

namespace Consumer;

public class LoadAddedConsumer : IConsumer<LoadAddedEvent>
{
    private readonly ILogger<LoadAddedConsumer> _logger;

    public LoadAddedConsumer(ILogger<LoadAddedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LoadAddedEvent> context)
    {
        _logger.LogInformation("Consume {LoadAddedEvent}", context.Message);

        await Task.CompletedTask;
    }
}