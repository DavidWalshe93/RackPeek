using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Laptops.Cpus;
using RackPeek.Domain.Resources.Hardware.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Laptops.Cpus;

public class LaptopCpuAddCommand(IServiceProvider provider)
    : AsyncCommand<LaptopCpuAddSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LaptopCpuAddSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<AddLaptopCpuUseCase>();

        var cpu = new Cpu
        {
            Model = settings.Model,
            Cores = settings.Cores,
            Threads = settings.Threads
        };

        await useCase.ExecuteAsync(settings.LaptopName, cpu);

        AnsiConsole.MarkupLine($"[green]CPU added to Laptop '{settings.LaptopName}'.[/]");
        return 0;
    }
}