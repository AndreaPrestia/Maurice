using Maurice.Domain.Entities;

namespace Maurice.Writer.Abstractions.Repositories;

public interface IEventEntityRepository
{
    Task InsertAsync<T>(T eventContent, EventTypeEntity eventType, CancellationToken cancellationToken) where T : class;
}