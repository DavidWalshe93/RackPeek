using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Gpu;

public class DesktopGpuRemoveSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandArgument(1, "<index>")]
    public int Index { get; set; }
}