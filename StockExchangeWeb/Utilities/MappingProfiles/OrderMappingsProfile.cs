using AutoMapper;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models;
using StockExchangeWeb.Models.Orders;

namespace StockExchangeWeb.Utilities.MappingProfiles
{
    public class OrderMappingsProfile : Profile 
    {
        public OrderMappingsProfile()
        {
            CreateMap<OrderWriteDTO, Order>();
        }
    }
}