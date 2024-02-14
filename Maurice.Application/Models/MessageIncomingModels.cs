namespace Maurice.Application.Models
{
    public record MessageIncomingInputModel(string Type, string Body);

    public record DispatcherInputModel(string IpAddress, string Hostname);
}
