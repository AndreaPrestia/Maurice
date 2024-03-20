using Maurice.Domain.Entities;

namespace Maurice.Scheduler.Abstractions.Repository;

public interface IEventTypeEntityRepository
{
    Task<EventTypeEntity?> GetAsync<T>(CancellationToken cancellationToken) where T : class;

    Task<List<EventTypeEntity>?> ReadAsync(CancellationToken cancellationToken);
}