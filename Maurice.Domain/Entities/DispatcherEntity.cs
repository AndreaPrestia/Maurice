namespace Maurice.Domain.Entities
{
    public class DispatcherEntity
    {
        public Guid Id { get; set; }
        public string IpAddress { get; set; } = null!;
        public string Hostname { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
    }
}
