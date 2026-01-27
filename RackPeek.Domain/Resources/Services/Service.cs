namespace RackPeek.Domain.Resources.Services;

public class Service : Resource
{
    public Network? Network { get; set; }
    public string? RunsOn { get; set; }
}

public class Network
{
    public string? Ip { get; set; }
    public int? Port { get; set; }
    public string? Protocol { get; set; }
    public string? Url { get; set; }
}