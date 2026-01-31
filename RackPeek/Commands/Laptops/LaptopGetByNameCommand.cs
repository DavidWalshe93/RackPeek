using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Laptops;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Laptops;

public class LaptopGetByNameCommand(IServiceProvider provider)
    : AsyncCommand<LaptopNameSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        LaptopNameSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<GetLaptopUseCase>();

        var Laptop = await useCase.ExecuteAsync(settings.Name);

        if (Laptop == null)
        {
            AnsiConsole.MarkupLine($"[red]Laptop '{settings.Name}' not found.[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]{Laptop.Name}[/]");
        return 0;
    }
}