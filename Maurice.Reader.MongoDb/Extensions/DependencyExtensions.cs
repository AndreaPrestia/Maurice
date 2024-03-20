using Maurice.Reader.Abstractions.Repository;
using Maurice.Reader.MongoDb.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Maurice.Reader.MongoDb.Extensions;

public static class DependencyExtensions
{
    public static void AddMongoReader(this IServiceCollection services)
    {
        services.AddScoped<IEventTypeEntityRepository, EventTypeEntityRepository>();
        services.AddScoped<IEventEntityRepository, EventEntityRepository>();
    }
}