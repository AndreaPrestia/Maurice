namespace Maurice.Domain.Entities
{
    public class MappingEntity
    {
        public Guid Id { get; set; }
        public string Topic { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
}
