using Confluent.Kafka;
using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using tasks.Models;
using tasks.Controllers;

namespace tasks.Kafka;

public class EventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public EventConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "$default",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("accounts-stream");

                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(stoppingToken);

                        var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountEvent>(consumeResult.Value ?? "");

                        if (kafkaEvent == null) { continue; }

                        using (IServiceScope scope = _serviceProvider.CreateScope())
                        {
                            AccountController accountController =
                                scope.ServiceProvider.GetService<AccountController>();

                            accountController.CreateOrUpdateAccount(kafkaEvent.Data);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    consumer.Close();
                }
            }
        });

        return Task.CompletedTask;
    }
}
