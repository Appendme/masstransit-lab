using ProtoBuf;

namespace Domain;

[ProtoContract]
public record LoadCompletedEvent : ILoadEvent
{
    [ProtoMember(1)]
    public Ulid Id { get; init; }
}