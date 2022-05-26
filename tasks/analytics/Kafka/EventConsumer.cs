using analytics.Controllers;
using analytics.Models;
using AutoMapper;
using Confluent.Kafka;
using proto.Tools;

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
        using IServiceScope scope = _serviceProvider.CreateScope();
        AccountController? accountController =
                scope.ServiceProvider.GetService<AccountController>();
        IMapper? mapper = scope.ServiceProvider.GetService<IMapper>();

        if (accountController == null || mapper == null) { return Task.CompletedTask; }

            Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "analytics-account"
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("account");

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();

                    if (consumeResult.Message.Headers.GetHeader("eventName") == "account-balance-changed")
                    {
                        var kafkaEvent = consumeResult.Message.Value.FromProtobufString<proto.BusEvent<proto.AccountBalanceChangedV1>>();

                        if (kafkaEvent == null) { continue; }

                        accountController.UpdateAccount(mapper.Map<Account>(kafkaEvent.Data));

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
        });


        Task.Run(() =>
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
                TransactionController? transactionController =
                    scope.ServiceProvider.GetService<TransactionController>();

            if (transactionController == null) { return; }

            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "analytics-transaction"
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe("transaction");

            try
            {
                while (true)
                {
                    var consumeResult = consumer.Consume();

                    if (consumeResult.Message.Headers.GetHeader("eventName") == "transaction-applied")
                    {
                        var kafkaEvent = consumeResult.Message.Value.FromProtobufString<proto.BusEvent<proto.TransactionAppliedV1>>();

                        if (kafkaEvent == null) { continue; }

                        transactionController.Save(mapper.Map<Transaction>(kafkaEvent.Data));

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
