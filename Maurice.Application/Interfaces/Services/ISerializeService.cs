using Maurice.Domain.Entities;

namespace Maurice.Application.Interfaces.Services
{
    public interface ISerializeService
    {
        Task<bool> SerializeAsync(EventEntity eventEntity, CancellationToken cancellationToken);
        Task<bool> SerializeAsync(RejectedEntity rejectedEntity, CancellationToken cancellationToken);
    }
}
