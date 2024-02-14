using Maurice.Domain.Entities;

namespace Maurice.Application.Interfaces.Services
{
    public interface IMappingService
    {
        Task<EventEntity> MapEventEntityAsync(DispatcherEntity dispatcherEntity, string json, CancellationToken cancellationToken);
    }
}
