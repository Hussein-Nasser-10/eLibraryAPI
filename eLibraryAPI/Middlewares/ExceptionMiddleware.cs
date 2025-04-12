using System.Net;
using System.Text.Json;

namespace eLibraryAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        //httpContext is the request itself passing through 
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Continue the pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new
            {
                message = exception.Message,
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            var payload = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(payload);
        }
    }
}
