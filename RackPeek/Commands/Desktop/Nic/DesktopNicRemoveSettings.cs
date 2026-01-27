using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Nics;

public class DesktopNicRemoveSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandArgument(1, "<index>")]
    public int Index { get; set; }
}