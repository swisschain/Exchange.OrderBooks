using Swisschain.Sdk.Server.WebApi.Pagination;

namespace OrderBooks.WebApi.Models.OrderBooks
{
    public class OrderBookRequestMany : PaginationRequest<string>
    {
        /// <summary>
        /// Asset pair symbol
        /// </summary>
        public string Symbol { get; set; }
    }
}
