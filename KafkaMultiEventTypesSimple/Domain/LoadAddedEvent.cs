using ProtoBuf;

namespace Domain;

[ProtoContract]
public record LoadAddedEvent : ILoadEvent
{
    [ProtoMember(1)]
    public Ulid Id { get; init; }
}