using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.TradedEntitiesService;

namespace StockExchangeWeb.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrdersController : Controller
    {
        private ISecuritiesProvider _securitiesProvider;
        private IMapper _mapper;
        private IStockExchange _stockExchange;

        public OrdersController(IMapper mapper, ISecuritiesProvider securitiesProvider
            , IStockExchange stockExchange)
        {
            _securitiesProvider = securitiesProvider;
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

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string ticker)
        {
            return Ok(_stockExchange.GetOrdersPlaced(ticker));
        }
    }
}