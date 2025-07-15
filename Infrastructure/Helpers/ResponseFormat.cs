namespace document_generator.Infrastructure.Helpers;

public class ResponseFormat
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public dynamic Data { get; set; } = null;
    public dynamic Errors { get; set; } = null;
}

public class ResponseFormat<T>
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; }
    public dynamic Errors { get; set; } = null;
}