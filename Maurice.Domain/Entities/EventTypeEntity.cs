namespace Maurice.Domain.Entities;

public class EventTypeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public long Created { get; set; }
    public long Updated { get; set; }
    public ICollection<string> Tags { get; set; } = new List<string>();
    public ICollection<ScheduleRule> ScheduleRules { get; set; } = new List<ScheduleRule>();
}

public class ScheduleRule
{
    public string Code { get; set; } = null!;
    public string Cron { get; set; } = null!;
    public string Type { get; set; } = null!;  
}