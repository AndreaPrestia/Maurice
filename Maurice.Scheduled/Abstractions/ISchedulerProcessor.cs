namespace Maurice.Scheduler.Abstractions
{
    public interface ISchedulerProcessor<T> where T : class
    {
        Task<bool> ProcessAsync(CancellationToken cancellationToken);
    }
}
