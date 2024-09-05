using Maurice.Writer.Abstractions.Repositories;
using Maurice.Writer.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Maurice.Writer.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddWriter(this IServiceCollection services)
    {
        services.AddScoped<IEventTypeEntityRepository, EventTypeEntityRepository>();
        services.AddScoped<IEventEntityRepository, EventEntityRepository>();
        services.AddScoped<IErrorEntityRepository, ErrorEntityRepository>();
        services.AddScoped<WriterProcessor>();
    }
}