using Maurice.Domain.Entities;

namespace Maurice.Reader.Abstractions.Repository;

public interface IEventEntityRepository
{
    Task<IList<EventEntity>?> ReadAsync(long start, long end, EventTypeEntity eventType, bool descending,
        CancellationToken cancellationToken);
}