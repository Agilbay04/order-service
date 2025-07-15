using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Order.Dtos;
using OrderService.Domain.Product.Dtos;
using OrderService.Domain.Product.Services;
using OrderService.Infrastructure.Helpers;
using System.Net;

namespace OrderService.Http.API.Version1.Products.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController(
        ProductService productService
    ) : ControllerBase
    {
        private readonly ProductService _productService = productService;

        [HttpGet("")]
        public async Task<ApiResponse> GetAll([FromQuery] ProductQueryDto param)
        {
            var listProduct = await _productService.GetAllProduct(param);
            return new ApiResponseData(HttpStatusCode.OK, listProduct);
        }

        [HttpGet("ids")]
        public async Task<ApiResponse> GetByIds([FromQuery] ProductParamDto param)
        {
            if (string.IsNullOrEmpty(param.ProductIds)) return new ApiResponseData(HttpStatusCode.BadRequest, "ProductIds is required");
            List<Guid> productIds = [.. param.ProductIds.Split(',').Select(Guid.Parse)];
            var listProduct = await _productService.GetProductByIds(productIds);
            return new ApiResponseData(HttpStatusCode.OK, listProduct);
        }
    }
}