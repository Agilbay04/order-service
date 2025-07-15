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
        public ApiResponseData<object> Get()
        {
            return new ApiResponseData<object>(System.Net.HttpStatusCode.OK, new { message = "Service is running, and Healty" });
        }
    }
}
