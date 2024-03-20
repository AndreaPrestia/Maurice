using Maurice.Domain.Entities;

namespace Maurice.Reader.Abstractions.Repository;

public interface IEventEntityRepository
{
    Task<IList<EventEntity?>?> ReadAsync(long start, long end, EventTypeEntity eventType, bool descending,
        CancellationToken cancellationToken);

    Task<IList<EventEntity?>?> ReadAsync(long start, long end, IList<EventTypeEntity> eventTypeEntities, bool descending,
        CancellationToken cancellationToken);
}