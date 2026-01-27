using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Nics;

public class DesktopNicAddCommand(IServiceProvider provider)
    : AsyncCommand<DesktopNicAddSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopNicAddSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<AddDesktopNicUseCase>();

        var nic = new Nic
        {
            Type = settings.Type,
            Speed = settings.Speed,
            Ports = settings.Ports
        };

        await useCase.ExecuteAsync(settings.DesktopName, nic);

        AnsiConsole.MarkupLine($"[green]NIC added to desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}