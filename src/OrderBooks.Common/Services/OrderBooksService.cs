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
        // nested dictionaries with two keys - BrokerId, Symbol
        private readonly ConcurrentDictionary<string, string, OrderBook> _orderBooks =
            new ConcurrentDictionary<string, string, OrderBook>();

        public IReadOnlyList<OrderBook> GetAll(string brokerId)
        {
            return _orderBooks.GetAll(brokerId);
        }

        public OrderBook Get(string brokerId, string symbol)
        {
            return _orderBooks.Get(brokerId, symbol);
        }
        
        public void Update(string brokerId, OrderBook orderBook)
        {
            var symbol = orderBook.Symbol;

            _orderBooks.Update(brokerId, symbol, orderBook);
        }

        public IReadOnlyList<OrderBook> GetAllAsync(string brokerId, string symbol,
            ListSortDirection sortOrder = ListSortDirection.Ascending, string cursor = null, int limit = 50)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return new List<OrderBook>();

            var brokersAllDict = _orderBooks[brokerId];

            var requiredKeys = brokersAllDict.Keys.Where(x => x.Contains(symbol)).ToList();

            var filteredOrderBooks = new List<OrderBook>();

            requiredKeys.ForEach(key => filteredOrderBooks.Add(brokersAllDict[key]));

            IEnumerable<OrderBook> query = filteredOrderBooks;

            if (sortOrder == ListSortDirection.Ascending)
            {
                if (cursor != null)
                    query = query.Where(x => string.Compare(x.Symbol, cursor, StringComparison.CurrentCultureIgnoreCase) >= 0);

                query = query.OrderBy(x => x.Symbol);
            }
            else
            {
                if (cursor != null)
                    query = query.Where(x => string.Compare(x.Symbol, cursor, StringComparison.CurrentCultureIgnoreCase) < 0);

                query = query.OrderByDescending(x => x.Symbol);
            }

            query = query.Take(limit);

            var result = query.ToList();

            return result;
        }
    }
}
