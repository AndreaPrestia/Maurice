using Maurice.Writer.Abstractions.Repositories;
using Maurice.Writer.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Maurice.Writer.Mongo.Extensions;

public static class DependencyExtensions
{
    public static void AddMongoWriter(this IServiceCollection services)
    {
        services.AddScoped<IEventTypeEntityRepository, EventTypeEntityRepository>();
        services.AddScoped<IEventEntityRepository, EventEntityRepository>();
        services.AddScoped<IErrorEntityRepository, ErrorEntityRepository>();
    }
}