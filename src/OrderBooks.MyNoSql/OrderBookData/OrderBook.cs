using System;
using System.Collections.Generic;

namespace OrderBooks.MyNoSql.OrderBookData
{
    /// <summary>
    /// Represents an order book.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The asset pair symbol.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The date and time of creation.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The collection of limit orders.
        /// </summary>
        public IReadOnlyList<LimitOrder> LimitOrders { get; set; }
    }
}
