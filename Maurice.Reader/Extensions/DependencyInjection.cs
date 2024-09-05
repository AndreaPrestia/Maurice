using System.Diagnostics.CodeAnalysis;
using Maurice.Reader.Abstractions.Repository;
using Maurice.Reader.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Maurice.Reader.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddReader(this IServiceCollection services)
    {
        services.AddScoped<IEventTypeEntityRepository, EventTypeEntityRepository>();
        services.AddScoped<IEventEntityRepository, EventEntityRepository>();
        services.AddScoped<IErrorEntityRepository, ErrorEntityRepository>();
        services.AddScoped<ReaderProcessor>();
    }
}