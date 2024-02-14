namespace Maurice.Domain.Entities
{
    public class TagEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public long Created { get; set; }
    }
}
