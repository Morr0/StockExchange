﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockExchangeWeb.Controllers.Bodies;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models;
using StockExchangeWeb.Models.Orders;
using StockExchangeWeb.Services;
using StockExchangeWeb.Services.ExchangeService;
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
            // Series of validations
            // BEGIN
            if (!_securitiesProvider.Securities.ContainsKey(orderWriteDto.Ticker))
                return BadRequest(new JsonResult("Ticker does not exist"));

            TradableSecurity security = _securitiesProvider.Securities[orderWriteDto.Ticker];
            if (orderWriteDto.Amount == 0)
                return BadRequest(new JsonResult("Amount of shares must not be 0"));
            else if (security.OutstandingAmount < orderWriteDto.Amount)
                return BadRequest(new JsonResult("Cannot ask for more shares than exists"));
            
            // END
            
            Order order = _mapper.Map<Order>(orderWriteDto);

            order = await _stockExchange.PlaceOrder(order);
            if (order == null)
                return BadRequest();

            return Ok(order);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder([FromBody] OrderDeletionBody deletionBody)
        {
            Order order = await _stockExchange.RemoveOrder(deletionBody.OrderDeletionKey);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] string ticker)
        {
            return Ok(_stockExchange.GetOrdersPlaced(ticker));
        }
    }
}