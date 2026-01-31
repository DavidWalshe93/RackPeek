using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Laptops.Gpus;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Laptops.Gpus;

public class LaptopGpuAddCommand(IServiceProvider provider)
    : AsyncCommand<LaptopGpuAddSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LaptopGpuAddSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<AddLaptopGpuUseCase>();

        var gpu = new Gpu
        {
            Model = settings.Model,
            Vram = settings.Vram
        };

        await useCase.ExecuteAsync(settings.LaptopName, gpu);

        AnsiConsole.MarkupLine($"[green]GPU added to Laptop '{settings.LaptopName}'.[/]");
        return 0;
    }
}