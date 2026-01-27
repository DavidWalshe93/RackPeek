using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktops.Gpus;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktops.Gpus;

public class DesktopGpuSetCommand(IServiceProvider provider)
    : AsyncCommand<DesktopGpuSetSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopGpuSetSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<UpdateDesktopGpuUseCase>();

        var gpu = new Gpu
        {
            Model = settings.Model,
            Vram = settings.Vram
        };

        await useCase.ExecuteAsync(settings.DesktopName, settings.Index, gpu);

        AnsiConsole.MarkupLine($"[green]GPU #{settings.Index} updated on desktop '{settings.DesktopName}'.[/]");
        return 0;
    }
}