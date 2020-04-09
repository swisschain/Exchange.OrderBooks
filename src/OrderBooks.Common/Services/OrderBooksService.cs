using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public IReadOnlyList<OrderBook> GetAllAsync(string brokerId, string assetPairId,
            ListSortDirection sortOrder = ListSortDirection.Ascending, string cursor = null, int limit = 50)
        {
            if (string.IsNullOrWhiteSpace(assetPairId))
                return new List<OrderBook>();

            var brokersAllDict = _orderBooks[brokerId];

            var requiredKeys = brokersAllDict.Keys.Where(x => x.Contains(assetPairId)).ToList();

            var filteredOrderBooks = new List<OrderBook>();

            requiredKeys.ForEach(key => filteredOrderBooks.Add(brokersAllDict[key]));

            IEnumerable<OrderBook> query = filteredOrderBooks;

            if (sortOrder == ListSortDirection.Ascending)
            {
                if (cursor != null)
                    query = query.Where(x => string.Compare(x.AssetPairId, cursor, StringComparison.CurrentCultureIgnoreCase) >= 0);

                query = query.OrderBy(x => x.AssetPairId);
            }
            else
            {
                if (cursor != null)
                    query = query.Where(x => string.Compare(x.AssetPairId, cursor, StringComparison.CurrentCultureIgnoreCase) < 0);

                query = query.OrderByDescending(x => x.AssetPairId);
            }

            query = query.Take(limit);

            var result = query.ToList();

            return result;
        }
    }
}
