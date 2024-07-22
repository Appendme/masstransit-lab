using ProtoBuf;

namespace Domain;

[ProtoContract]
[ProtoInclude(1, typeof(LoadAddedEvent))]
[ProtoInclude(2, typeof(LoadStartedEvent))]
[ProtoInclude(3, typeof(LoadProgressEvent))]
[ProtoInclude(4, typeof(LoadCompletedEvent))]
public interface ILoadEvent
{
    public Ulid Id { get; init; }
}