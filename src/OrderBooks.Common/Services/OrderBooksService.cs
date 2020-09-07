using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MyNoSqlServer.Abstractions;
using OrderBooks.Common.Domain.Services;
using OrderBooks.Common.Utils;
using OrderBooks.MyNoSql;
using OrderBooks.MyNoSql.OrderBookData;

namespace OrderBooks.Common.Services
{
    public class OrderBooksService : IOrderBooksService
    {
        private readonly IMyNoSqlServerDataWriter<OrderBookEntity> _dataWriter;

        // nested dictionaries with two keys - BrokerId, Symbol
        private readonly ConcurrentDictionary<string, string, OrderBook> _orderBooks =
            new ConcurrentDictionary<string, string, OrderBook>();

        public OrderBooksService(IMyNoSqlServerDataWriter<OrderBookEntity> dataWriter)
        {
            _dataWriter = dataWriter;
        }

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

            var entity = OrderBookEntity.GenerateEntity(brokerId, orderBook.Symbol);
            entity.OrderBook = orderBook;

            _dataWriter.InsertOrReplaceAsync(entity).GetAwaiter().GetResult();
        }

        public IReadOnlyList<OrderBook> GetAllAsync(string brokerId, string symbol,
            ListSortDirection sortOrder = ListSortDirection.Ascending, string cursor = null, int limit = 50)
        {
            var allOrderBooks = _orderBooks.GetAll(brokerId);

            IEnumerable<OrderBook> query = allOrderBooks;

            if (!string.IsNullOrWhiteSpace(symbol))
                query = query.Where(x => x.Symbol.Contains(symbol));

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
