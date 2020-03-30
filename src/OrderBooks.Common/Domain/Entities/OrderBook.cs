using System;
using System.Collections.Generic;

namespace OrderBooks.Common.Domain.Entities
{
    /// <summary>
    /// Represents an order book.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The asset pair identifier.
        /// </summary>
        public string AssetPairId { get; set; }

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
