using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Drives;

public class DesktopDriveSetCommand(IServiceProvider provider)
    : AsyncCommand<DesktopDriveSetSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopDriveSetSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<UpdateDesktopDriveUseCase>();

        var drive = new Drive
        {
            Type = settings.Type,
            Size = settings.Size
        };

        await useCase.ExecuteAsync(settings.DesktopName, settings.Index, drive);

        AnsiConsole.MarkupLine($"[green]Drive #{settings.Index} updated on desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}