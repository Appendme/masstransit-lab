using Domain;
using MassTransit;
using Polly;

namespace Producer;

public class ProducerWorker : BackgroundService
{
    private readonly ILogger<ProducerWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ProducerWorker(ILogger<ProducerWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(15, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await ProduceProcess(stoppingToken));
    }

    private async Task ProduceProcess(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<string, ILoadEvent>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var ulid = Ulid.NewUlid();
            var ulidString = ulid.ToString();

            await producer.Produce(ulidString, new LoadAddedEvent { Id = ulid }, stoppingToken);
            await RandomDelay();

            await producer.Produce(ulidString, new LoadStartedEvent { Id = ulid }, stoppingToken);
            await RandomDelay();

            for (var i = 0; i <= 100; i += Random.Shared.Next(1, 20))
            {
                await producer.Produce(ulidString, new LoadProgressEvent { Id = ulid, ProgressPercentage = i },
                    stoppingToken);
                await RandomDelay();
            }

            await producer.Produce(ulidString, new LoadProgressEvent { Id = ulid, ProgressPercentage = 100 },
                stoppingToken);
            await RandomDelay();

            await producer.Produce(ulidString, new LoadCompletedEvent { Id = ulid }, stoppingToken);
            await RandomDelay();
        }
    }

    private static async Task RandomDelay(int min = 1000, int max = 3000)
    {
        await Task.Delay(Random.Shared.Next(min, max));
    }
}