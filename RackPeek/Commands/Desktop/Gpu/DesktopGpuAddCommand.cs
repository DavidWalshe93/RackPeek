using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop.Gpu;

public class DesktopGpuAddCommand(IServiceProvider provider)
    : AsyncCommand<DesktopGpuAddSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopGpuAddSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<AddDesktopGpuUseCase>();

        var gpu = new Domain.Resources.Hardware.Models.Gpu
        {
            Model = settings.Model,
            Vram = settings.Vram
        };

        await useCase.ExecuteAsync(settings.DesktopName, gpu);

        AnsiConsole.MarkupLine($"[green]GPU added to desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}