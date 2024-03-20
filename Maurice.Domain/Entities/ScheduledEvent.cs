namespace Maurice.Domain.Entities
{
    public sealed class ScheduledEvent
    {
        public Guid Id { get; set; }
        public long Timestamp { get; set; }
        public string Body { get; set; } = null!;
        public ScheduleRule ScheduleRule { get; set; } = null!;
    }
}
