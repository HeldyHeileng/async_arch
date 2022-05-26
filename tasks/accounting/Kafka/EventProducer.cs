using Confluent.Kafka;
using accounting.Models;

namespace accounting.Kafka;

public class EventProducer
{
    public async Task Produce(string topic, string eventName, object eventData) {

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "$default",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            
        };

        var _event = new BusEvent<object>() { 
            EventId = Guid.NewGuid(),
            EventName = eventName,
            Producer = "tasks",
            ProducedAt = DateTime.UtcNow,
            Data = eventData,
            Version = 1
        };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            var dataString = Newtonsoft.Json.JsonConvert.SerializeObject(_event);
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = dataString });
        }
    }
}
