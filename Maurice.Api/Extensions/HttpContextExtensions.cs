using Maurice.Domain.Entities;

namespace Maurice.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static DispatcherEntity GetDispatcherFromHttpContext(this HttpContext httpContext)
        {
            return new DispatcherEntity()
            {
                Hostname = httpContext.Request.Host.Value,
                IpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ??
                            throw new InvalidOperationException("No ip address provided")
            };
        }
    }
}
