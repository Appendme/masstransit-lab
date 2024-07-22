using ProtoBuf;

namespace Domain;

[ProtoContract]
public record LoadProgressEvent : ILoadEvent
{
    [ProtoMember(1)]
    public Ulid Id { get; init; }
    
    [ProtoMember(2)]
    public int ProgressPercentage { get; init; }
}