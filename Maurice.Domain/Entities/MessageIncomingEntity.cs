namespace Maurice.Domain.Entities
{
    public class MessageIncomingEntity
    {
        public Guid Id { get; set; }
        public string Body { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DispatcherEntity Dispatcher { get; set; } = null!;
        public long Timestamp { get; set; }
    }
}
