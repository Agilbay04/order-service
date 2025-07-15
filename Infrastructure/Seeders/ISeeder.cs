using OrderService.Infrastructure.Databases;

namespace OrderService.Infrastructure.Seeders
{
  public interface ISeeder
  {
    Task Seed(DataContext dbContext, ILogger logger);
  }
}
