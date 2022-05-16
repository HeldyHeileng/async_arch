using Confluent.Kafka;

namespace tasks.Kafka;

public class EventProducer
{
    public async Task Produce(string topic, object eventData) {

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "$default",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            
        };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            var dataString = Newtonsoft.Json.JsonConvert.SerializeObject(eventData);
            await producer.ProduceAsync(topic, new Message<Null, string> { Value = dataString });
        }
    }
}
