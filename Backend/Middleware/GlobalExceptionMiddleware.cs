using Backend.Exceptions;

namespace Backend.Middleware
{
  public class GlobalExceptionMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception err)
      {
        await HandleExceptionAsync(context, err);
      }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      var errorId = Guid.NewGuid().ToString("N").ToUpper();
      _logger.LogError(exception, "ERROR TIDAK DITANGANI [{ErrorId}]: {Message}", errorId, exception.Message);

      context.Response.ContentType = "application/json";
      context.Response.StatusCode = 
        exception is NotFoundException ? StatusCodes.Status404NotFound :
        exception is BadRequestException ? StatusCodes.Status400BadRequest :
        StatusCodes.Status500InternalServerError;

      var response = new ErrorResponse
      {
        Success = false,
        Error = new ErrorDetail
        {
          Type = exception.GetType().Name,
          Message = exception.Message,
          TraceId = errorId
        }
      };

      await context.Response.WriteAsJsonAsync(response);
    }


    public class ErrorResponse
    {
      public bool Success {get;set;}
      public ErrorDetail? Error {get;set;}
    }

    public class ErrorDetail
    {
      public string Type {get;set;} = string.Empty;
      public string Message {get;set;} = string.Empty;
      public string TraceId {get;set;} = string.Empty;
    }
  }
}