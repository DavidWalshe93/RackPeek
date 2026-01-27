using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Cpus;

public class DesktopCpuAddSettings : CommandSettings
{
    [CommandArgument(0, "<desktop>")]
    public string DesktopName { get; set; } = default!;

    [CommandOption("--model")]
    public string? Model { get; set; }

    [CommandOption("--cores")]
    public int? Cores { get; set; }

    [CommandOption("--threads")]
    public int? Threads { get; set; }
}