using System;
using System.Linq;
using System.Threading.Tasks;
using MatchingEngine.Client;
using OrderBooks.Common.Domain.Entities;
using OrderBooks.Common.Domain.Handlers;
using OrderBooks.RabbitMq.Subscribers;

namespace OrderBooks.Managers
{
    public class StartupManager
    {
        private readonly OrderBooksSubscriber _orderBooksSubscriber;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IOrderBooksHandler _orderBooksHandler;

        public StartupManager(
            OrderBooksSubscriber orderBooksSubscriber,
            IMatchingEngineClient matchingEngineClient,
            IOrderBooksHandler orderBooksHandler)
        {
            _orderBooksSubscriber = orderBooksSubscriber;
            _matchingEngineClient = matchingEngineClient;
            _orderBooksHandler = orderBooksHandler;
        }

        public async Task StartAsync()
        {
            var orderBooks = await _matchingEngineClient.OrderBooks.GetAllAsync();

            foreach (var orderBook in orderBooks)
            {
                var type = orderBook.IsBuy
                    ? LimitOrderType.Buy
                    : LimitOrderType.Sell;

                var limitOrders = orderBook.Levels
                    .Select(level => new LimitOrder
                    {
                        Id = Guid.Parse(level.OrderId),
                        Price = decimal.Parse(level.Price),
                        Volume = Math.Abs(decimal.Parse(level.Volume)),
                        WalletId = level.WalletId,
                        Type = type
                    })
                    .ToList();

                if (orderBook.IsBuy)
                    _orderBooksHandler.HandleBuy(orderBook.Asset, orderBook.Timestamp.ToDateTime(), limitOrders);
                else
                    _orderBooksHandler.HandleSell(orderBook.Asset, orderBook.Timestamp.ToDateTime(), limitOrders);
            }

            _orderBooksSubscriber.Start();
        }
    }
}
