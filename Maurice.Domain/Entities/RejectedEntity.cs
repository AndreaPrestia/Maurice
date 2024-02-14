﻿namespace Maurice.Domain.Entities
{
    public class RejectedEntity
    {
        public Guid Id { get; set; }
        public long Created { get; set; }
        public string Body { get; set; } = null!;
        public string Topic { get; set; } = null!;
        public DispatcherEntity Dispatcher { get; set; } = null!;
    }
}
