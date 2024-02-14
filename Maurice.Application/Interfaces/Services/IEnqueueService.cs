using Maurice.Domain.Entities;

namespace Maurice.Application.Interfaces.Services
{
    public interface IEnqueueService
    {
        Task<bool> EnqueueAsync(DispatcherEntity dispatcherEntity, string requestBody, CancellationToken cancellationToken);
    }
}
