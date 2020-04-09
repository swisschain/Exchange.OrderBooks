using System.Collections.Generic;
using OrderBooks.Common.Domain.Entities;
using OrderBooks.Common.Domain.Services;
using OrderBooks.Common.Utils;

namespace OrderBooks.Common.Services
{
    public class OrderBooksService : IOrderBooksService
    {
        // nested dictionaries with two keys - BrokerId, AssetPairId
        private readonly ConcurrentDictionary<string, string, OrderBook> _orderBooks =
            new ConcurrentDictionary<string, string, OrderBook>();

        public IReadOnlyList<OrderBook> GetAll(string brokerId)
        {
            return _orderBooks.GetAll(brokerId);
        }

        public OrderBook Get(string brokerId, string assetPairId)
        {
            return _orderBooks.Get(brokerId, assetPairId);
        }
        
        public void Update(string brokerId, OrderBook orderBook)
        {
            var assetPairId = orderBook.AssetPairId;

            _orderBooks.Update(brokerId, assetPairId, orderBook);
        }
    }
}
