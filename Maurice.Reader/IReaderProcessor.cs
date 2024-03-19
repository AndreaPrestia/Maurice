namespace Maurice.Reader;

public interface IReaderProcessor<T> where T : class
{
    Task<IList<T>> ReadAsync(Func<T, bool> predicate, CancellationToken cancellationToken);
    Task<IList<T>> ReadAsync(long start, long end, CancellationToken cancellationToken);
}