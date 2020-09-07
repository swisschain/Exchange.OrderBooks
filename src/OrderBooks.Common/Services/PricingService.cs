using System;
using MyNoSqlServer.Abstractions;
using OrderBooks.Common.Domain.Services;
using OrderBooks.Common.Utils;
using OrderBooks.MyNoSql.PriceData;

namespace OrderBooks.Common.Services
{
    public class PricingService: IPricingService
    {
        private readonly IMyNoSqlServerDataWriter<PriceEntity> _dataWriter;

        private readonly ConcurrentDictionary<string, string, PriceEntity> _prices =
            new ConcurrentDictionary<string, string, PriceEntity>();

        public PricingService(IMyNoSqlServerDataWriter<PriceEntity> dataWriter)
        {
            _dataWriter = dataWriter;
        }

        public void Update(string brokerId,
            string symbol,
            in DateTime timestamp,
            decimal? ask,
            decimal? bid)
        {
            if (!_prices.TryGetValue(brokerId, out var brokerData) || !brokerData.TryGetValue(symbol, out var price))
            {
                price = PriceEntity.GenerateEntity(brokerId, symbol);
                price.LastUpdate = DateTime.MinValue;
            }

            if (price.LastUpdate > timestamp)
            {
                return;
            }

            price.LastUpdate = timestamp;
            price.Ask = ask;
            price.Bid = bid;
            
            _dataWriter.InsertOrReplaceAsync(price).GetAwaiter().GetResult();
        }
    }
}
