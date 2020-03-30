using System;
using System.Collections.Generic;
using OrderBooks.Common.Domain.Entities;

namespace OrderBooks.Common.Domain.Handlers
{
    public interface IOrderBooksHandler
    {
        void HandleSell(string assetPairId, DateTime timestamp, IReadOnlyList<LimitOrder> limitOrders);

        void HandleBuy(string assetPairId, DateTime timestamp, IReadOnlyList<LimitOrder> limitOrders);
    }
}
