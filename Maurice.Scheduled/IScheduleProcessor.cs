namespace Maurice.Scheduled
{
    public interface IScheduleProcessor
    {
        Task<bool> ScheduleAsync(CancellationToken cancellationToken);
    }
}
