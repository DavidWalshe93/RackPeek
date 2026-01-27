using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Nics;

public class DesktopNicAddSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandOption("--type")]
    public string? Type { get; set; }

    [CommandOption("--speed")]
    public int? Speed { get; set; }

    [CommandOption("--ports")]
    public int? Ports { get; set; }
}