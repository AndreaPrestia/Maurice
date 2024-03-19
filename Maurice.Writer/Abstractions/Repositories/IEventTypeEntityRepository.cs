using Maurice.Domain.Entities;

namespace Maurice.Writer.Abstractions.Repositories;

public interface IEventTypeEntityRepository
{
    Task<EventTypeEntity?> GetAsync<T>(CancellationToken cancellationToken) where T : class;
}