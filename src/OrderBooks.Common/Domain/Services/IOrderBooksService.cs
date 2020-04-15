using System.Collections.Generic;
using System.ComponentModel;
using OrderBooks.Common.Domain.Entities;

namespace OrderBooks.Common.Domain.Services
{
    public interface IOrderBooksService
    {
        IReadOnlyList<OrderBook> GetAll(string brokerId);

        IReadOnlyList<OrderBook> GetAllAsync(string brokerId, string symbol,
            ListSortDirection sortOrder = ListSortDirection.Ascending, string cursor = null, int limit = 50);

        OrderBook Get(string brokerId, string symbol);

        void Update(string brokerId, OrderBook orderBook);
    }
}
