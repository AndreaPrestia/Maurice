using Maurice.Reader.Abstractions.Repository;
using Maurice.Reader.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Maurice.Reader.Mongo.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyExtensions
{
    public static void AddMongoReader(this IServiceCollection services)
    {
        services.AddScoped<IEventTypeEntityRepository, EventTypeEntityRepository>();
        services.AddScoped<IEventEntityRepository, EventEntityRepository>();
        services.AddScoped<IErrorEntityRepository, ErrorEntityRepository>();
    }
}