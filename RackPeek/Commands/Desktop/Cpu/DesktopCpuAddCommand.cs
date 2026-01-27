using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Cpus;

public class DesktopCpuAddCommand(IServiceProvider provider)
    : AsyncCommand<DesktopCpuAddSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopCpuAddSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<AddDesktopCpuUseCase>();

        var cpu = new Cpu
        {
            Model = settings.Model,
            Cores = settings.Cores,
            Threads = settings.Threads
        };

        await useCase.ExecuteAsync(settings.DesktopName, cpu);

        AnsiConsole.MarkupLine($"[green]CPU added to desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}