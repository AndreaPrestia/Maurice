using Maurice.Domain.Entities;

namespace Maurice.Reader.Abstractions.Repository;

public interface IEventTypeEntityRepository
{
    Task<EventTypeEntity?> GetAsync<T>(CancellationToken cancellationToken) where T : class;
}