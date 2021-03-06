using accounting.Controllers;
using accounting.Models;
using Confluent.Kafka;


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
        Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "accounting",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("tasks");

                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(stoppingToken);

                        var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<BusEvent<TrackerTask>>(consumeResult.Value ?? "");

                        if (kafkaEvent == null) { continue; }

                        if (kafkaEvent.EventName == "task-added")
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                TaskEventHandlers accountController =
                                    scope.ServiceProvider.GetService<TaskEventHandlers>();

                                accountController.TaskAddedHandler(kafkaEvent.Data);
                            }
                            continue;
                        }

                        if (kafkaEvent.EventName == "task-completed")
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                TaskEventHandlers accountController =
                                    scope.ServiceProvider.GetService<TaskEventHandlers>();

                                accountController.TaskCompletedHandler(kafkaEvent.Data);
                            }
                            continue;
                        }

                        if (kafkaEvent.EventName == "task-shuffled")
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                TaskEventHandlers accountController =
                                    scope.ServiceProvider.GetService<TaskEventHandlers>();

                                accountController.TaskShuffledHandler(kafkaEvent.Data);
                            }
                            continue;
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

        Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "accounting",
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

                        var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<BusEvent<Account>>(consumeResult.Value ?? "");

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
