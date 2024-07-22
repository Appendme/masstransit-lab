using ProtoBuf;

namespace Domain;

[ProtoContract]
public record LoadStartedEvent : ILoadEvent
{
    [ProtoMember(1)]
    public Ulid Id { get; init; }
}