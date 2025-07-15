using System.Text.Json;

namespace OrderService.Infrastructure.Helpers
{
  public static class JsonSerializeSeeder
  {
    public static JsonSerializerOptions options { get; set; } = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
      PropertyNameCaseInsensitive = true
    };
  }
}