using System;

namespace OrderBooks.Common.Domain.Services
{
    public interface IPricingService
    {
        void Update(string brokerId,
            string symbol,
            in DateTime timestamp,
            decimal? ask,
            decimal? bid);
    }
}
