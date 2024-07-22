using Common;
using Consumer;
using Domain;
using MassTransit;
using Microsoft.Extensions.Options;
using ProtoBuf.Meta;
using Serializers;

RuntimeTypeModel.Default.AddUlid();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));

builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options => { options.WaitUntilStarted = true; });
builder.Services.AddMassTransit(configurator =>
{
    // register IBus
    configurator.UsingInMemory();

    configurator.AddRider(riderConfig =>
    {
        riderConfig.AddConsumer<LoadEventConsumer>();
        riderConfig.AddConsumer<LoadAddedConsumer>();

        riderConfig.UsingKafka((riderContext, kafkaConfig) =>
        {
            var kafkaOptions = riderContext.GetRequiredService<IOptions<KafkaOptions>>().Value;
            
            kafkaConfig.Host(kafkaOptions.Host);
            kafkaConfig.TopicEndpoint<string, ILoadEvent>("load-events", "consumer-group-name", topicConfig =>
            {
                topicConfig.SetValueDeserializer(new ProtobufSerializer<ILoadEvent>());

                topicConfig.ConfigureConsumer<LoadEventConsumer>(riderContext);
                topicConfig.ConfigureConsumer<LoadAddedConsumer>(riderContext);
            });
        });
    });
});

var host = builder.Build();
host.Run();