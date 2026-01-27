using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Gpu;

public class DesktopGpuAddSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandOption("--model")]
    public string? Model { get; set; }

    [CommandOption("--vram")]
    public int? Vram { get; set; }
}