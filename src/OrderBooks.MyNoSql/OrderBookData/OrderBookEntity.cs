using System;
using MyNoSqlServer.Abstractions;

namespace OrderBooks.MyNoSql.OrderBookData
{
    public class OrderBookEntity : IMyNoSqlDbEntity
    {
        public const string OrderBookTableName = "order-books";

        public OrderBook OrderBook { get; set; }

        public static string GeneratePartitionKey(string brokerId) => brokerId;
        public static string GenerateRowKey(string symbol) => symbol;

        public static OrderBookEntity GenerateEntity(string brokerId, string symbol)
        {
            var entity = new OrderBookEntity()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(symbol)
            };

            return entity;
        }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}
