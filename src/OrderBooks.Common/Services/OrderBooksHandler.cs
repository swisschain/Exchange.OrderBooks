using System;
using System.Collections.Generic;
using System.Linq;
using OrderBooks.Common.Domain.Handlers;
using OrderBooks.Common.Domain.Services;
using OrderBooks.Common.Utils;
using OrderBooks.MyNoSql.OrderBookData;

namespace OrderBooks.Common.Services
{
    public class OrderBooksHandler : IOrderBooksHandler
    {
        private readonly ConcurrentDictionary<string, string, OrderBookInfo> _dirtyOrderBooks =
            new ConcurrentDictionary<string, string, OrderBookInfo>();

        // can be removed if OrderBookInfo properties are 'volatile'
        private readonly object _sync = new object();

        private readonly IOrderBooksService _orderBooksService;
        private readonly IPricingService _pricingService;

        public OrderBooksHandler(IOrderBooksService orderBooksService, IPricingService pricingService)
        {
            _orderBooksService = orderBooksService;
            _pricingService = pricingService;
        }

        public void Handle(string brokerId, string symbol, bool isBuy, DateTime timestamp, IReadOnlyList<LimitOrder> limitOrders)
        {
            lock (_sync)
            {
                var existed = _dirtyOrderBooks.Get(brokerId, symbol);

                if (existed != null)
                {
                    existed.Timestamp = timestamp;

                    if (isBuy)
                        existed.BuyLimitOrders = limitOrders;
                    else
                        existed.SellLimitOrders = limitOrders;

                    UpdateInOrderBookService(brokerId, existed);
                }
                else
                {
                    var newOrderBookInfo = new OrderBookInfo
                    {
                        Symbol = symbol,
                        Timestamp = timestamp
                    };

                    if (isBuy)
                        newOrderBookInfo.BuyLimitOrders = limitOrders;
                    else
                        newOrderBookInfo.SellLimitOrders = limitOrders;

                    _dirtyOrderBooks.Update(brokerId, symbol, newOrderBookInfo);
                }
            }
        }

        private void UpdateInOrderBookService(string brokerId, OrderBookInfo orderBookInfo)
        {
            if (orderBookInfo.SellLimitOrders != null && orderBookInfo.BuyLimitOrders != null)
            {
                var isValid = true;

                if (orderBookInfo.SellLimitOrders.Any() && orderBookInfo.BuyLimitOrders.Any())
                {
                    isValid = orderBookInfo.SellLimitOrders.Min(o => o.Price) >
                              orderBookInfo.BuyLimitOrders.Max(o => o.Price);
                }

                if (!isValid)
                {
                    return;
                }
            }

            var orders = new List<LimitOrder>();

            if (orderBookInfo.SellLimitOrders != null && orderBookInfo.SellLimitOrders.Any())
            {
                orders.AddRange(orderBookInfo.SellLimitOrders.OrderByDescending(o => o.Price));
            }

            if (orderBookInfo.BuyLimitOrders != null && orderBookInfo.BuyLimitOrders.Any())
            {
                orders.AddRange(orderBookInfo.BuyLimitOrders.OrderBy(o => o.Price));
            }

            _orderBooksService.Update(brokerId, new OrderBook
            {
                Symbol = orderBookInfo.Symbol,
                Timestamp = orderBookInfo.Timestamp,
                LimitOrders = orders
            });
        }

        private void UpdateInPricingService(string brokerId, OrderBookInfo orderBookInfo)
        {
            decimal? ask = null;
            decimal? bid = null;

            if (orderBookInfo.SellLimitOrders != null && orderBookInfo.SellLimitOrders.Any())
            {
                ask = orderBookInfo.SellLimitOrders.Min(o => o.Price);
            }

            if (orderBookInfo.BuyLimitOrders != null && orderBookInfo.BuyLimitOrders.Any())
            {
                bid = orderBookInfo.BuyLimitOrders.Max(o => o.Price);
            }


            if (ask.HasValue && bid.HasValue && ask.Value <= bid.Value)
            {
                return;
            }

            _pricingService.Update(brokerId, orderBookInfo.Symbol, orderBookInfo.Timestamp, ask, bid);
        }

        private class OrderBookInfo
        {
            public string Symbol { get; set; }

            public DateTime Timestamp { get; set; }

            public IReadOnlyList<LimitOrder> SellLimitOrders { get; set; }

            public IReadOnlyList<LimitOrder> BuyLimitOrders { get; set; }
        }
    }
}
