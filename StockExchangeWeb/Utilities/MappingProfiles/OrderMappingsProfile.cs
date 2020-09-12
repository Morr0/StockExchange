using AutoMapper;
using StockExchangeWeb.DTOs;
using StockExchangeWeb.Models;

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