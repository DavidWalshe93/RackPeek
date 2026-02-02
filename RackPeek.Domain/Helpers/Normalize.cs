namespace RackPeek.Domain.Helpers;

public static class Normalize
{
    public static string DriveType(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
    public static string NicType(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}