using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Drives;

public class DesktopDriveAddSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandOption("--type")]
    public string? Type { get; set; }

    [CommandOption("--size")]
    public int? Size { get; set; }
}