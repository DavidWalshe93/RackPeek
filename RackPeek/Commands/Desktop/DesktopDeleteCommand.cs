using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Hardware.Desktop;
using Spectre.Console;
using Spectre.Console.Cli;

namespace RackPeek.Commands.Desktop;

public class DesktopDeleteCommand(IServiceProvider provider)
    : AsyncCommand<DesktopNameSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        DesktopNameSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<DeleteDesktopUseCase>();

        await useCase.ExecuteAsync(settings.Name);

        AnsiConsole.MarkupLine($"[green]Desktop '{settings.Name}' deleted.[/]");
        return 0;
    }
}