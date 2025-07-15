using OrderService.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OrderService.Http.API.Health
{
    [Route("order-service/health")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController
    {
        [HttpGet]
        public ApiResponseData Get()
        {
            return new ApiResponseData(
                System.Net.HttpStatusCode.OK,
                new { message = "Order Service is running, and Healty" }
            );
        }
    }
}
