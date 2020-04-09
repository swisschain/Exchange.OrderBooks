using System.Collections.Generic;
using OrderBooks.Common.Domain.Entities;

namespace OrderBooks.Common.Domain.Services
{
    public interface IOrderBooksService
    {
        IReadOnlyList<OrderBook> GetAll(string brokerId);

        OrderBook Get(string brokerId, string assetPairId);

        void Update(string brokerId, OrderBook orderBook);
    }
}
