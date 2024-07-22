using Common;
using Domain;
using MassTransit;
using Microsoft.Extensions.Options;
using Producer;
using ProtoBuf.Meta;
using Serializers;

RuntimeTypeModel.Default.AddUlid();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ProducerWorker>();

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));

builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options => { options.WaitUntilStarted = true; });
builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingInMemory();
    configurator.AddRider(riderConfig =>
    {
        riderConfig.AddProducer<string, ILoadEvent>("load-events", (_, producerConfig) =>
        {
            producerConfig.SetValueSerializer(new ProtobufSerializer<ILoadEvent>());
        });

        riderConfig.UsingKafka((riderContext, kafkaConfig) =>
        {
            var kafkaOptions = riderContext.GetRequiredService<IOptions<KafkaOptions>>().Value;
            
            kafkaConfig.Host(kafkaOptions.Host);
        });
    });
});

var host = builder.Build();
host.Run();