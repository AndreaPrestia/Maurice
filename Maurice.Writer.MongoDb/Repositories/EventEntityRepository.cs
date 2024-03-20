using Maurice.Domain.Entities;
using Maurice.Writer.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Text.Json;

namespace Maurice.Writer.MongoDb.Repositories;

public sealed class EventEntityRepository : IEventEntityRepository
{
    private readonly IMongoCollection<EventEntity> _eventCollection;

    public EventEntityRepository(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var client = new MongoClient(configuration.GetConnectionString("maurice"));
        _eventCollection = client.GetDatabase("maurice").GetCollection<EventEntity>("events");
    }

    public async Task InsertAsync<T>(T eventContent, EventTypeEntity eventType, CancellationToken cancellationToken) where T : class
    {
        var entity = new EventEntity()
        {
            Body = JsonSerializer.Serialize(eventContent),
            EventTypeId = eventType.Id,
            Id = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
        };

        await _eventCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
}