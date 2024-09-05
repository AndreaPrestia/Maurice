using Maurice.Domain.Entities;

namespace Maurice.Application.Interfaces.Services
{
	public interface IDispatcherService
	{
		Task<DispatcherEntity> GetDispatcherAsync(string apiKey, CancellationToken cancellationToken);
	}
}
