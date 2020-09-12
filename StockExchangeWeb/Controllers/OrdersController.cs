using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models;
using StockExchangeWeb.Services;

namespace StockExchangeWeb.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrdersController : Controller
    {
        private IMapper _mapper;
        private IStockExchange _stockExchange;

        public OrdersController(IMapper mapper, IStockExchange stockExchange)
        {
            _mapper = mapper;
            _stockExchange = stockExchange;
        }
        
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderWriteDTO orderWriteDto)
        {
            Order order = _mapper.Map<Order>(orderWriteDto);

            order = _stockExchange.PlaceOrder(order);
            if (order == null)
                return BadRequest();

            return Ok(order);
        }
    }
}