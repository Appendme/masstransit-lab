using Confluent.Kafka;
using ProtoBuf;
using SerializationContext = Confluent.Kafka.SerializationContext;

namespace Serializers;

public class ProtobufSerializer<T> : ISerializer<T>, IDeserializer<T> where T : class
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty)
            // todo when is possible?
            throw new ArgumentNullException(nameof(data));

        return Serializer.Deserialize<T>(data);
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        using var memoryStream = new MemoryStream();
        Serializer.Serialize(memoryStream, data);
        return memoryStream.ToArray();
    }
}