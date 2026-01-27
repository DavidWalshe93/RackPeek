using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Nics;

public class DesktopNicSetCommand(IServiceProvider provider)
    : AsyncCommand<DesktopNicSetSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopNicSetSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<UpdateDesktopNicUseCase>();

        var nic = new Nic
        {
            Type = settings.Type,
            Speed = settings.Speed,
            Ports = settings.Ports
        };

        await useCase.ExecuteAsync(settings.DesktopName, settings.Index, nic);

        AnsiConsole.MarkupLine($"[green]NIC #{settings.Index} updated on desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}