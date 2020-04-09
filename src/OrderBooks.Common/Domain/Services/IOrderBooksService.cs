using System.Collections.Generic;
using System.ComponentModel;
using OrderBooks.Common.Domain.Entities;

namespace OrderBooks.Common.Domain.Services
{
    public interface IOrderBooksService
    {
        IReadOnlyList<OrderBook> GetAll(string brokerId);

        IReadOnlyList<OrderBook> GetAllAsync(string brokerId, string assetPairId,
            ListSortDirection sortOrder = ListSortDirection.Ascending, string cursor = null, int limit = 50);

        OrderBook Get(string brokerId, string assetPairId);

        void Update(string brokerId, OrderBook orderBook);
    }
}
