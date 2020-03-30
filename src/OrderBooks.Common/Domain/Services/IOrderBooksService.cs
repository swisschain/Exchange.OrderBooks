using System.Collections.Generic;
using OrderBooks.Common.Domain.Entities;

namespace OrderBooks.Common.Domain.Services
{
    public interface IOrderBooksService
    {
        IReadOnlyList<OrderBook> GetAll();

        OrderBook GetByAssetPairId(string assetPairId);

        void Update(OrderBook orderBook);
    }
}
