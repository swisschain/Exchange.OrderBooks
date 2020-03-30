using System;
using System.Collections.Generic;
using System.Text;

namespace OrderBooks.Common.Domain.Entities
{
    /// <summary>
    /// Represents a limit order details.
    /// </summary>
    public class LimitOrder
    {
        /// <summary>
        /// The identifier of the limit order.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The limit order price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The limit order volume.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The wallet identifier.
        /// </summary>
        public string WalletId { get; set; }

        /// <summary>
        /// The limit order type.
        /// </summary>
        public LimitOrderType Type { get; set; }
    }
}
