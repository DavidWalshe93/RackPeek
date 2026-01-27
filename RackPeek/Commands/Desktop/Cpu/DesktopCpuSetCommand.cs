using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Cpus;

public class DesktopCpuSetCommand(IServiceProvider provider)
    : AsyncCommand<DesktopCpuSetSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopCpuSetSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<UpdateDesktopCpuUseCase>();

        var cpu = new Cpu
        {
            Model = settings.Model,
            Cores = settings.Cores,
            Threads = settings.Threads
        };

        await useCase.ExecuteAsync(settings.DesktopName, settings.Index, cpu);

        AnsiConsole.MarkupLine($"[green]CPU #{settings.Index} updated on desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}