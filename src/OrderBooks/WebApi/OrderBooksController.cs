using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderBooks.Common.Domain.Services;
using OrderBooks.WebApi.Models.OrderBooks;
using Swisschain.Sdk.Server.Authorization;
using Swisschain.Sdk.Server.WebApi.Common;
using Swisschain.Sdk.Server.WebApi.Pagination;

namespace OrderBooks.WebApi
{
    [Authorize]
    [ApiController]
    [Route("api/order-books")]
    public class OrderBooksController : ControllerBase
    {
        private readonly IOrderBooksService _orderBooksService;
        private readonly IMapper _mapper;

        public OrderBooksController(IOrderBooksService orderBooksService, IMapper mapper)
        {
            _orderBooksService = orderBooksService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Paginated<OrderBookModel, string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ModelStateDictionaryErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] OrderBookRequestMany request)
        {
            var sortOrder = request.Order == PaginationOrder.Asc
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

            var brokerId = User.GetTenantId();

            var accounts = _orderBooksService.GetAllAsync(brokerId, request.Symbol, sortOrder, request.Cursor, request.Limit);

            var result = _mapper.Map<OrderBookModel[]>(accounts);

            return Ok(result.Paginate(request, Url, x => x.Symbol));
        }

        [HttpGet("{symbol}")]
        [ProducesResponseType(typeof(OrderBookModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAsync(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return NotFound();

            var brokerId = User.GetTenantId();

            var orderBook = _orderBooksService.Get(brokerId, symbol);

            if (orderBook == null)
                return NotFound();

            var model = _mapper.Map<OrderBookModel>(orderBook);

            return Ok(model);
        }
    }
}
