using accounting.Controllers;
using accounting.Models;
using AutoMapper;
using Confluent.Kafka;
using proto.Tools;
using System.Text;

namespace accounting.Kafka;

public class EventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public EventConsumer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(async () =>
        {
            using IServiceScope scope = _serviceProvider.CreateScope();            
            TaskEventHandlers? accountController =
                scope.ServiceProvider.GetService<TaskEventHandlers>();

            if (accountController == null) { return; }

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "accounting"
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("tasks");
            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();

                    if (consumeResult.Message.Headers.GetHeader("eventName") == "task-added")
                    {
                        var kafkaEvent = consumeResult.Message.Value.FromProtobufString<proto.BusEvent<proto.TaskAddedV2>>();

                        if (kafkaEvent == null) { continue; }

                        await accountController.TaskAddedHandler(kafkaEvent.Data);

                        continue;
                    }

                    if (consumeResult.Message.Headers.GetHeader("eventName") == "task-completed")
                    {
                        var kafkaEvent = consumeResult.Message.Value.FromProtobufString<proto.BusEvent<proto.TaskCompletedV1>>();

                        if (kafkaEvent == null) { continue; }

                        await accountController.TaskCompletedHandler(kafkaEvent.Data);
                        continue;
                    }

                    if (consumeResult.Message.Headers.GetHeader("eventName") == "task-shuffled")
                    {
                        var kafkaEvent = consumeResult.Message.Value.FromProtobufString<proto.BusEvent<proto.TaskShuffledV1>>();

                        if (kafkaEvent == null) { continue; }

                        await accountController.TaskShuffledHandler(kafkaEvent.Data);
                        continue;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
                consumer.Close();
            }
        }, stoppingToken);

        Task.Run(() =>
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            AccountController? accountController =
                scope.ServiceProvider.GetService<AccountController>();

            if (accountController == null) { return; }

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "accounting",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("accounts-stream");

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();

                    var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<BusEvent<Account>>(consumeResult.Message.Value ?? "");

                    if (kafkaEvent == null) { continue; }

                    accountController.CreateOrUpdateAccount(kafkaEvent.Data);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                consumer.Close();
            }
        }, stoppingToken);
        return Task.CompletedTask;
    }
}
