using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktops.Drives;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktops.Drive;

public class DesktopDriveAddCommand(IServiceProvider provider)
    : AsyncCommand<DesktopDriveAddSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopDriveAddSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<AddDesktopDriveUseCase>();

        var drive = new Domain.Resources.Hardware.Models.Drive
        {
            Type = settings.Type,
            Size = settings.Size
        };

        await useCase.ExecuteAsync(settings.DesktopName, drive);

        AnsiConsole.MarkupLine($"[green]Drive added to desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}