using System;
using System.Text.Json;
using Confluent.Kafka;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Kafka;

public class SystemTextJsonDeserializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions _serializerOptions;

    public SystemTextJsonDeserializer(JsonSerializerOptions serializerOptions)
    {
        _serializerOptions = serializerOptions;
    }


    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            throw new ArgumentNullException(nameof(isNull));
        }

        return JsonSerializer.Deserialize<T>(data, _serializerOptions) ?? throw new InvalidOperationException();
    }
}