namespace Maurice.Domain.Entities
{
    public class EventEntity 
    {
        public Guid Id { get; set; }
        public long Created { get; set; }
        public string Body { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public string Type { get; set; } = null!;
        public DispatcherEntity Dispatcher { get; set; } = null!;
        public ICollection<TagEntity> Tags { get; set; } = new List<TagEntity>();
    }
}
