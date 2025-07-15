using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Order.Services;
using OrderService.Domain.Product.Dtos;
using OrderService.Infrastructure.Helpers;
using System.Net;

namespace OrderService.Http.API.Version1.Order.Controllers
{
    [Route("api/v1/orders")]
    [ApiController]
    [AllowAnonymous]
    public class OrderController(
        ServiceOrder serviceOrder 
    ) : ControllerBase
    {
        private readonly ServiceOrder _serviceOrder = serviceOrder;

        [HttpGet("")]
        public async Task<ApiResponse> GetAll([FromQuery] ProductQueryDto param)
        {
            var listProduct = await _serviceOrder.GetAllProduct(param);
            return new ApiResponseData(HttpStatusCode.OK, listProduct);
        }
    }
}