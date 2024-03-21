using Maurice.Domain.Entities;
using Maurice.Writer.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Text.Json;

namespace Maurice.Writer.Mongo.Repositories;

public class ErrorEntityRepository : IErrorEntityRepository
{
    private readonly IMongoCollection<ErrorEntity> _errorCollection;

    public ErrorEntityRepository(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var client = new MongoClient(configuration.GetConnectionString("maurice"));
        _errorCollection = client.GetDatabase("maurice").GetCollection<ErrorEntity>("errors");
    }

    public async Task InsertAsync<T>(T? eventContent, Exception exception, CancellationToken cancellationToken) where T : class
    {
        var entity = new ErrorEntity()
        {
            Body = eventContent != null ? JsonSerializer.Serialize(eventContent) : "{}",
            Error = exception.Message,
            Id = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()
        };

        await _errorCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
}