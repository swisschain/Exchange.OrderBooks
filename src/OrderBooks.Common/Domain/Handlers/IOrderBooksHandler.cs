using System;
using System.Collections.Generic;
using OrderBooks.MyNoSql.OrderBookData;

namespace OrderBooks.Common.Domain.Handlers
{
    public interface IOrderBooksHandler
    {
        void Handle(string brokerId, string symbol, bool isBuy, DateTime timestamp, IReadOnlyList<LimitOrder> limitOrders);
    }
}
