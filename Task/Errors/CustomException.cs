using Microsoft.AspNetCore.Diagnostics;

namespace Task.Errors
{
    public class CustomException : IExceptionHandler
    {
        private readonly ILogger<CustomException> logger;
        public CustomException(ILogger<CustomException> logger)
        {
            this.logger = logger;
        }
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var exception_message = exception.Message;
            logger.LogError("Error: {exception_message}, Time: {time}", exception_message, DateTime.UtcNow);
            return ValueTask.FromResult(false); // true after handling
        }
    }
}
