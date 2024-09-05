using Maurice.Domain.Entities;
using Maurice.Reader.Abstractions.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Maurice.Reader.Repositories;

public class EventTypeEntityRepository : IEventTypeEntityRepository
{
    private readonly IMongoCollection<EventTypeEntity> _eventTypeCollection;
    private readonly ILogger<EventTypeEntityRepository> _logger;

    public EventTypeEntityRepository(IConfiguration configuration, ILogger<EventTypeEntityRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        var client = new MongoClient(configuration.GetConnectionString("maurice"));
        _eventTypeCollection = client.GetDatabase("maurice").GetCollection<EventTypeEntity>("event-types");
    }

    public async Task<EventTypeEntity?> GetAsync<T>(CancellationToken cancellationToken) where T : class
    {
        try
        {
            var type = typeof(T);

            var filter = Builders<EventTypeEntity>.Filter
                .Eq(r => r.Name, type.Name);

            return await _eventTypeCollection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<IList<EventTypeEntity>?> ReadAsync(IList<string> tags, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<EventTypeEntity>.Filter.In("Tags", tags);

            return await _eventTypeCollection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }
}