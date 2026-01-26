namespace RackPeek.Domain.Resources;

public abstract class Resource
{
    public string Kind { get; set; } = string.Empty;

    public required string Name { get; set; }

    public Dictionary<string, string>? Tags { get; set; }
}