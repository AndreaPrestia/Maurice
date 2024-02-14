using Maurice.Application.Models;

namespace Maurice.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static DispatcherInputModel GetDispatcherFromHttpContext(this HttpContext httpContext)
        {
            return new DispatcherInputModel(httpContext.Connection.RemoteIpAddress?.ToString() ??
                                            throw new InvalidOperationException("No ip address provided"),
                httpContext.Request.Host.Value);
        }
    }
}
