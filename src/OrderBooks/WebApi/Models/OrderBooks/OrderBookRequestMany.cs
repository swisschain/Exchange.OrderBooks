using Swisschain.Sdk.Server.WebApi.Pagination;

namespace OrderBooks.WebApi.Models.OrderBooks
{
    public class OrderBookRequestMany : PaginationRequest<string>
    {
        /// <summary>
        /// Asset pair identifier
        /// </summary>
        public string AssetPairId { get; set; }
    }
}
