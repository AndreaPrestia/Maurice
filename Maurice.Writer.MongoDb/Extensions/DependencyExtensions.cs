using Maurice.Writer.Abstractions.Repositories;
using Maurice.Writer.MongoDb.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Maurice.Writer.MongoDb.Extensions;

public static class DependencyExtensions
{
    public static void AddMongoWriter(this IServiceCollection services)
    {
        services.AddScoped<IEventTypeEntityRepository, EventTypeEntityRepository>();
        services.AddScoped<IEventEntityRepository, EventEntityRepository>();
    }
}