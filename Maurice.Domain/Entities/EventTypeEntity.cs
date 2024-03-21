namespace Maurice.Domain.Entities;

public sealed class EventTypeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public long Created { get; set; }
    public long Updated { get; set; }
    public IList<string> Tags { get; set; } = new List<string>();
}
