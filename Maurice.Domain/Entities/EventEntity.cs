﻿namespace Maurice.Domain.Entities;

public class EventEntity 
{
    public Guid Id { get; set; }
    public long Timestamp { get; set; }
    public string Body { get; set; } = null!;
    public Guid EventTypeId { get; set; }
}