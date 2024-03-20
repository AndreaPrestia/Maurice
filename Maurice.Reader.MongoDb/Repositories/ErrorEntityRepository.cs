using Maurice.Domain.Entities;
using Maurice.Reader.Abstractions.Repository;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Maurice.Reader.MongoDb.Repositories
{
    public class ErrorEntityRepository : IErrorEntityRepository
    {
        private readonly IMongoCollection<ErrorEntity> _errorCollection;

        public ErrorEntityRepository(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            var client = new MongoClient(configuration.GetConnectionString("maurice"));
            _errorCollection = client.GetDatabase("maurice").GetCollection<ErrorEntity>("errors");
        }

        public async Task<IList<ErrorEntity>?> ReadAsync(long start, long end, bool descending, CancellationToken cancellationToken)
        {
            var builder = Builders<ErrorEntity>.Filter;

            var filter = builder.Gte(r => r.Timestamp, start)
                         & builder.Lte(r => r.Timestamp, end);

            var sortBuilder = Builders<ErrorEntity>.Sort;

            var sort = descending ? sortBuilder.Descending(f => f.Timestamp) : sortBuilder.Ascending(f => f.Timestamp);

            return await _errorCollection.Find(filter).Sort(sort).ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
