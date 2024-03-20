using Maurice.Domain.Entities;

namespace Maurice.Reader.Abstractions.Repository
{
    public interface IErrorEntityRepository
    {
        Task<IList<ErrorEntity>?> ReadAsync(long start, long end, bool descending, CancellationToken cancellationToken);
    }
}
