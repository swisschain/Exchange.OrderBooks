using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderBooks.Common.Domain.Services;
using OrderBooks.WebApi.Models.OrderBooks;

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
        [ProducesResponseType(typeof(OrderBookModel[]), StatusCodes.Status200OK)]
        public IActionResult GetAllAsync()
        {
            var orderBooks = _orderBooksService.GetAll();

            var model = _mapper.Map<List<OrderBookModel>>(orderBooks);

            return Ok(model);
        }

        [HttpGet("{assetPairId}")]
        [ProducesResponseType(typeof(OrderBookModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByAssetPairIdAsync(string assetPairId)
        {
            if (string.IsNullOrWhiteSpace(assetPairId))
                return NotFound();

            var orderBook = _orderBooksService.GetByAssetPairId(assetPairId);

            if (orderBook == null)
                return NotFound();

            var model = _mapper.Map<OrderBookModel>(orderBook);

            return Ok(model);
        }
    }
}
