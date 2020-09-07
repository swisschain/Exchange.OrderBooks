using AutoMapper;
using OrderBooks.MyNoSql.OrderBookData;
using OrderBooks.WebApi.Models.OrderBooks;

namespace OrderBooks
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<OrderBook, OrderBookModel>(MemberList.Destination);

            CreateMap<LimitOrder, LimitOrderModel>(MemberList.Destination);
        }
    }
}
