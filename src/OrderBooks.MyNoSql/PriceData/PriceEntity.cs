using System;
using MyNoSqlServer.Abstractions;
using OrderBooks.MyNoSql.OrderBookData;

namespace OrderBooks.MyNoSql.PriceData
{
    public class PriceEntity: IMyNoSqlDbEntity
    {
        public const string PriceTableName = "price-current";

        public string BrokerId { get; set; }

        public string Symbol { get; set; }

        public DateTime LastUpdate { get; set; }

        public decimal? Ask { get; set; }

        public decimal? Bid { get; set; }

        public static string GeneratePartitionKey(string brokerId) => brokerId;
        public static string GenerateRowKey(string symbol) => symbol;

        public static PriceEntity GenerateEntity(string brokerId, string symbol)
        {
            var entity = new PriceEntity()
            {
                PartitionKey = GeneratePartitionKey(brokerId),
                RowKey = GenerateRowKey(symbol),
                Symbol = symbol,
                BrokerId = brokerId
            };

            return entity;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}
