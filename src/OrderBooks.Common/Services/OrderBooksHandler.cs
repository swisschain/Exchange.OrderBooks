using System;
using System.Collections.Generic;
using System.Linq;
using OrderBooks.Common.Domain.Entities;
using OrderBooks.Common.Domain.Handlers;
using OrderBooks.Common.Domain.Services;

namespace OrderBooks.Common.Services
{
    public class OrderBooksHandler : IOrderBooksHandler
    {
        private readonly object _sync = new object();

        private readonly Dictionary<string, OrderBookInfo> _dirtyOrderBooks =
            new Dictionary<string, OrderBookInfo>();

        private readonly IOrderBooksService _orderBooksService;

        public OrderBooksHandler(IOrderBooksService orderBooksService)
        {
            _orderBooksService = orderBooksService;
        }

        public void HandleSell(string assetPairId, DateTime timestamp, IReadOnlyList<LimitOrder> limitOrders)
        {
            lock (_sync)
            {
                if (!_dirtyOrderBooks.TryGetValue(assetPairId, out var orderBookInfo))
                {
                    _dirtyOrderBooks.Add(assetPairId,
                        new OrderBookInfo
                        {
                            AssetPairId = assetPairId, Timestamp = timestamp, SellLimitOrders = limitOrders
                        });
                }
                else
                {
                    orderBookInfo.Timestamp = timestamp;
                    orderBookInfo.SellLimitOrders = limitOrders;

                    Handle(orderBookInfo);
                }
            }
        }

        public void HandleBuy(string assetPairId, DateTime timestamp, IReadOnlyList<LimitOrder> limitOrders)
        {
            lock (_sync)
            {
                if (!_dirtyOrderBooks.TryGetValue(assetPairId, out var orderBookInfo))
                {
                    _dirtyOrderBooks.Add(assetPairId,
                        new OrderBookInfo
                        {
                            AssetPairId = assetPairId, Timestamp = timestamp, BuyLimitOrders = limitOrders
                        });
                }
                else
                {
                    orderBookInfo.Timestamp = timestamp;
                    orderBookInfo.BuyLimitOrders = limitOrders;

                    Handle(orderBookInfo);
                }
            }
        }

        private void Handle(OrderBookInfo orderBookInfo)
        {
            if (orderBookInfo.SellLimitOrders != null && orderBookInfo.BuyLimitOrders != null)
            {
                var isValid = true;

                if (orderBookInfo.SellLimitOrders.Any() && orderBookInfo.BuyLimitOrders.Any())
                {
                    isValid = orderBookInfo.SellLimitOrders.Min(o => o.Price) >
                              orderBookInfo.BuyLimitOrders.Max(o => o.Price);
                }

                if (isValid)
                {
                    _orderBooksService.Update(new OrderBook
                    {
                        AssetPairId = orderBookInfo.AssetPairId,
                        Timestamp = orderBookInfo.Timestamp,
                        LimitOrders = orderBookInfo.SellLimitOrders
                            .OrderByDescending(o => o.Price)
                            .Union(orderBookInfo.BuyLimitOrders.OrderBy(o => o.Price))
                            .ToList()
                    });
                }
            }
        }

        private class OrderBookInfo
        {
            public string AssetPairId { get; set; }

            public DateTime Timestamp { get; set; }

            public IReadOnlyList<LimitOrder> SellLimitOrders { get; set; }

            public IReadOnlyList<LimitOrder> BuyLimitOrders { get; set; }
        }
    }
}
