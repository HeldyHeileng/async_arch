using analytics.Controllers;
using analytics.Models;
using Confluent.Kafka;


namespace analytics.Kafka;

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
                GroupId = "analytics-account"
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
            consumer.Subscribe("account");

            try
            {
                while (true)
                {
                        var consumeResult = consumer.Consume(stoppingToken);

                        var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<BusEvent<Account>>(consumeResult.Value ?? "");

                        if (kafkaEvent == null) { continue; }

                        if (kafkaEvent.EventName == "account-balance-changed")
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                AccountController accountController =
                                    scope.ServiceProvider.GetService<AccountController>();

                                accountController.UpdateAccount(kafkaEvent.Data);
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
                GroupId = "analytics-transaction"
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
            consumer.Subscribe("transaction");

            try
            {
                while (true)
                {
                        var consumeResult = consumer.Consume(stoppingToken);

                        var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<BusEvent<Transaction>>(consumeResult.Value ?? "");

                        if (kafkaEvent == null) { continue; }

                        if (kafkaEvent.EventName == "transaction-added")
                        {
                            using (IServiceScope scope = _serviceProvider.CreateScope())
                            {
                                TransactionController transactionController =
                                    scope.ServiceProvider.GetService<TransactionController>();

                                transactionController.Save(kafkaEvent.Data);
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

            using IServiceScope scope = _serviceProvider.CreateScope();
            AccountController? accountController =
                    scope.ServiceProvider.GetService<AccountController>();

            if (accountController == null) { return; }
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "analytics-accounts-stream",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("accounts-stream");

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();

                    var kafkaEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<BusEvent<Account>>(consumeResult.Message.Value);

                    if (kafkaEvent == null) { continue; }
                    accountController.CreateAccount(kafkaEvent.Data);

                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                consumer.Close();
            }
        });
        return Task.CompletedTask;
    }
}
