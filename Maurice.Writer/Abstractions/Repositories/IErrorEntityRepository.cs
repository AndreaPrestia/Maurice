using Maurice.Domain.Entities;

namespace Maurice.Writer.Abstractions.Repositories
{
    public interface IErrorEntityRepository
    {
        Task InsertAsync<T>(T eventContent, Exception exception, CancellationToken cancellationToken) where T : class;
    }
}
