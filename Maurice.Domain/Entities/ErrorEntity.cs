namespace Maurice.Domain.Entities;

public sealed class ErrorEntity
{
    public Guid Id { get; set; }
    public long Timestamp { get; set; }
    public string Body { get; set; } = null!;
    public string Error { get; set; } = null!;
}