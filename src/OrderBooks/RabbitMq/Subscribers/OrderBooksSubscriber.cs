using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using MatchingEngine.Client.Contracts.Outgoing;
using Microsoft.Extensions.Logging;
using OrderBooks.Common.Domain.Handlers;
using OrderBooks.Configuration.Service.RabbitMq.Subscribers;
using OrderBooks.MyNoSql.OrderBookData;
using Swisschain.LykkeLog.Adapter;

namespace OrderBooks.RabbitMq.Subscribers
{
    public class OrderBooksSubscriber
    {
        private readonly SubscriberSettings _settings;
        private readonly IOrderBooksHandler _orderBooksHandler;
        private readonly ILogger<OrderBooksSubscriber> _logger;

        private RabbitMqSubscriber<OrderBookSnapshotEvent> _subscriber;

        public OrderBooksSubscriber(
            SubscriberSettings settings,
            IOrderBooksHandler orderBooksHandler,
            ILogger<OrderBooksSubscriber> logger)
        {
            _settings = settings;
            _orderBooksHandler = orderBooksHandler;
            _logger = logger;
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _settings.ConnectionString,
                ExchangeName = _settings.Exchange,
                QueueName = $"{_settings.Exchange}.{_settings.QueueSuffix}",
                DeadLetterExchangeName = null,
                IsDurable = false
            };

            _subscriber = new RabbitMqSubscriber<OrderBookSnapshotEvent>(LegacyLykkeLogFactoryToConsole.Instance,
                    settings,
                    new ResilientErrorHandlingStrategy(LegacyLykkeLogFactoryToConsole.Instance, settings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new GoogleProtobufMessageDeserializer<OrderBookSnapshotEvent>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private Task ProcessMessageAsync(OrderBookSnapshotEvent message)
        {
            try
            {
                var brokerId = message.OrderBook.BrokerId;
                var symbol = message.OrderBook.Asset;
                var timestamp = message.OrderBook.Timestamp.ToDateTime();
                var limitOrders = message.OrderBook.Levels
                    .Select(level => new LimitOrder
                    {
                        Id = Guid.Parse(level.OrderId),
                        Price = decimal.Parse(level.Price),
                        Volume = Math.Abs(decimal.Parse(level.Volume)),
                        WalletId = level.WalletId,
                        Type = message.OrderBook.IsBuy
                            ? LimitOrderType.Buy
                            : LimitOrderType.Sell
                    })
                    .ToList();

                _orderBooksHandler.Handle(brokerId, symbol, message.OrderBook.IsBuy, timestamp, limitOrders);

                _logger.LogInformation("Received an Order Book. {@OrderBook}", message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred during processing order book. {@Message}",  message);
            }

            return Task.CompletedTask;
        }
    }
}
