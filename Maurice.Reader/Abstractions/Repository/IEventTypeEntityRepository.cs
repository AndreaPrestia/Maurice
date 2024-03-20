using Maurice.Domain.Entities;

namespace Maurice.Reader.Abstractions.Repository;

public interface IEventTypeEntityRepository
{
    Task<EventTypeEntity?> GetAsync<T>(CancellationToken cancellationToken) where T : class;
    Task<IList<EventTypeEntity>?> ReadAsync(IList<string> tags, CancellationToken cancellationToken);
}