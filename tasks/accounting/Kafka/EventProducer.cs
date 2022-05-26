using Confluent.Kafka;
using accounting.Models;
using AutoMapper;
using proto.Tools;
using System.Text;

namespace accounting.Kafka;

public class EventProducer
{
    private readonly IMapper _mapper;

    public EventProducer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Produce<T>(string topic, string eventName, object eventData)
    {

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        var _event = new proto.BusEvent<T>()
        {
            EventId = Guid.NewGuid(),
            EventName = eventName,
            Producer = "accounting",
            ProducedAt = DateTime.UtcNow,
            Data = _mapper.Map<T>(eventData),
            Version = 1
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();
        var headers = new Headers();
        headers.Add("eventName", Encoding.UTF8.GetBytes(eventName));

        await producer.ProduceAsync(topic, new Message<Null, string> { 
            Value = _event.ToProtobufString(),
            Headers = headers
        });
    }
}
