using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Nics;

public class DesktopNicSetSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandArgument(1, "<index>")]
    public int Index { get; set; }

    [CommandOption("--type")]
    public string? Type { get; set; }

    [CommandOption("--speed")]
    public int? Speed { get; set; }

    [CommandOption("--ports")]
    public int? Ports { get; set; }
}