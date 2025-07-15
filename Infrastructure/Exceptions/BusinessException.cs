namespace OrderService.Infrastructure.Exceptions
{
    public class BusinessException(string message) : Exception(message)
    {
    }
}