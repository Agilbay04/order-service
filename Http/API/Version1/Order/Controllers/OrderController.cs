using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Order.Dtos;
using OrderService.Domain.Order.Messages;
using OrderService.Domain.Order.Services;
using OrderService.Infrastructure.Helpers;
using System.Net;

namespace OrderService.Http.API.Version1.Order.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [AllowAnonymous]
    public class OrderController(
        IServiceOrder serviceOrder 
    ) : ControllerBase
    {
        private readonly IServiceOrder _serviceOrder = serviceOrder;

        [HttpGet()]
        public async Task<ApiResponse> Index([FromQuery] OrderQueryDto param)
        {
            var listOrder = await _serviceOrder.FindAllAsync(param);
            return new ApiResponsePagination<OrderResultDto>(HttpStatusCode.OK, listOrder);
        }

        [HttpPost()]
        public async Task<ApiResponse> CreateOrder([FromBody] CreateOrderDto body)
        {
            var createOrder = await _serviceOrder.CreateOrder(body);
            return new ApiResponseData(HttpStatusCode.OK, createOrder, OrderMessage.SuccessCreateOrder(createOrder));
        }
    }
}