using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Drives;

public class DesktopDriveSetSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandArgument(1, "<index>")]
    public int Index { get; set; }

    [CommandOption("--type")]
    public string? Type { get; set; }

    [CommandOption("--size")]
    public int? Size { get; set; }
}