using System.Text.Json;
using Maurice.Domain.Entities;
using Maurice.Writer.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Maurice.Writer.MongoDb.Repositories;

public sealed class EventEntityRepository : IEventEntityRepository
{
    private readonly IMongoCollection<EventEntity> _eventCollection;
    private readonly ILogger<EventEntityRepository> _logger;

    public EventEntityRepository(IConfiguration configuration, ILogger<EventEntityRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
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