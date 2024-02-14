using Maurice.Application.Models;
using Maurice.Domain.Entities;

namespace Maurice.Application.Interfaces.Services
{
    public interface IEnqueueService
    {
        Task<bool> EnqueueAsync(DispatcherInputModel dispatcher, MessageIncomingInputModel message, CancellationToken cancellationToken);
    }
}
