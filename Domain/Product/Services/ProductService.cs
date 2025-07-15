using document_generator.Infrastructure.Helpers;
using OrderService.Constants.Event;
using OrderService.Constants.Logger;
using OrderService.Domain.Product.Dtos;
using OrderService.Infrastructure.Helpers;
using OrderService.Infrastructure.Integrations.NATs;
using OrderService.Infrastructure.Shareds;
using System.Net;

namespace OrderService.Domain.Product.Services
{
    public class ProductService(
        NATsIntegration natsIntegration,
        ILoggerFactory loggerFactory
    )
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        private readonly NATsIntegration _natsIntegration = natsIntegration;

        public async Task<List<ProductResultDto>> GetAllProduct(ProductQueryDto param)
        {
            var listProduct = new List<ProductResultDto>();

            try
            {
                // Create subject for Get All Product
                string inventorySubject = _natsIntegration.Subject(
                    NATsEventModuleEnum.PRODUCT,
                    NATsEventActionEnum.GET,
                    NATsEventStatusEnum.REQUEST
                );

                // Publish and get reply from NATs Get All Product
                var invetoryReply = await _natsIntegration.PublishAndGetReply<object, object>(
                    inventorySubject,
                    Utils.JsonSerialize(new ApiResponseData(HttpStatusCode.OK, param))
                );

                _logger.LogInformation("Get reply Data from Invetory Service {invoiceReply}", invetoryReply);


                if (invetoryReply != null)
                {
                    var productRes = Utils.ParseFromJsonObject<ResponseFormat<List<ProductResultDto>>>(invetoryReply);
                    listProduct.AddRange(productRes.Data);
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                _logger.LogError(ex, "Error reply Data from Invetory Service {errMessage}", errMessage);
            }

            return listProduct;
        }

        public async Task<List<ProductResultDto>> GetProductByIds(List<Guid> productIds)
        {
            var listProduct = new List<ProductResultDto>();

            try
            {
                // Create subject for Get Data Product By IDs
                string inventorySubject = _natsIntegration.Subject(
                    NATsEventModuleEnum.PRODUCT,
                    NATsEventActionEnum.GET_BY_IDS,
                    NATsEventStatusEnum.REQUEST
                );

                // Publish and get reply from NATs Get Data Product By IDs
                var invetoryReply = await _natsIntegration.PublishAndGetReply<object, object>(
                    inventorySubject,
                    Utils.JsonSerialize(new ApiResponseData(HttpStatusCode.OK, productIds))
                );

                _logger.LogInformation("Get reply Data from Invetory Service {invoiceReply}", invetoryReply);

                if (invetoryReply != null)
                {
                    var productRes = Utils.ParseFromJsonObject<ResponseFormat<List<ProductResultDto>>>(invetoryReply);
                    listProduct.AddRange(productRes.Data);
                }
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                _logger.LogError(ex, "Error reply Data from Invetory Service {errMessage}", errMessage);
            }

            return listProduct;
        }
    }
}