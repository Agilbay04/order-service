namespace OrderService.Infrastructure.Exceptions
{
    public class ServiceUnavailableException(string message = "ServiceUnavailableException") : Exception(message)
    {
    }
}