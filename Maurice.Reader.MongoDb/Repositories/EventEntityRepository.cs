using Maurice.Domain.Entities;
using Maurice.Reader.Abstractions.Repository;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Maurice.Reader.MongoDb.Repositories;

public sealed class EventEntityRepository : IEventEntityRepository
{
    private readonly IMongoCollection<EventEntity> _eventCollection;

    public EventEntityRepository(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var client = new MongoClient(configuration.GetConnectionString("maurice"));
        _eventCollection = client.GetDatabase("maurice").GetCollection<EventEntity>("events");
    }

    public async Task<IList<EventEntity>?> ReadAsync(long start, long end, EventTypeEntity eventType, bool descending, CancellationToken cancellationToken)
    {
        var builder = Builders<EventEntity>.Filter;

        var filter = builder.Gte(r => r.Timestamp, start) 
                     & builder.Lte(r => r.Timestamp, end) 
                     & builder.Eq(r => r.EventTypeId, eventType.Id);

        var sortBuilder = Builders<EventEntity>.Sort;

        var sort = descending ? sortBuilder.Descending(f => f.Timestamp) : sortBuilder.Ascending(f => f.Timestamp);

        return await _eventCollection.Find(filter).Sort(sort).ToListAsync(cancellationToken: cancellationToken);
    }
}