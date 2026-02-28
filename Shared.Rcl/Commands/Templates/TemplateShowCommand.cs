using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using RackPeek.Domain.Resources.Firewalls;
using RackPeek.Domain.Resources.Routers;
using RackPeek.Domain.Resources.SubResources;
using RackPeek.Domain.Resources.Switches;
using RackPeek.Domain.Resources.AccessPoints;
using RackPeek.Domain.Resources.UpsUnits;
using RackPeek.Domain.Templates;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Shared.Rcl.Commands.Templates;

public class TemplateShowSettings : CommandSettings
{
    [CommandArgument(0, "<template-id>")]
    [Description("Template identifier in Kind/Model format (e.g. Switch/UniFi-USW-Enterprise-24).")]
    public string TemplateId { get; set; } = default!;
}

public class TemplateShowCommand(IServiceProvider provider)
    : AsyncCommand<TemplateShowSettings>
{
    public override async Task<int> ExecuteAsync(
        CommandContext context,
        TemplateShowSettings settings,
        CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IHardwareTemplateStore>();

        var template = await store.GetByIdAsync(settings.TemplateId);
        if (template is null)
        {
            AnsiConsole.MarkupLine($"[red]Template '{settings.TemplateId}' not found.[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[bold]{template.Id}[/]");
        AnsiConsole.MarkupLine($"  Kind:  {template.Kind}");
        AnsiConsole.MarkupLine($"  Model: {template.Model}");

        var spec = template.Spec;

        switch (spec)
        {
            case Switch sw:
                RenderSwitch(sw);
                break;
            case Router rt:
                RenderPortDevice(rt.Model, rt.Managed, rt.Poe, rt.Ports);
                break;
            case Firewall fw:
                RenderPortDevice(fw.Model, fw.Managed, fw.Poe, fw.Ports);
                break;
            case AccessPoint ap:
                if (ap.Speed.HasValue)
                    AnsiConsole.MarkupLine($"  Speed: {ap.Speed}G");
                break;
            case RackPeek.Domain.Resources.UpsUnits.Ups ups:
                if (ups.Va.HasValue)
                    AnsiConsole.MarkupLine($"  VA:    {ups.Va}");
                break;
        }

        return 0;
    }

    private static void RenderSwitch(Switch sw)
    {
        RenderPortDevice(sw.Model, sw.Managed, sw.Poe, sw.Ports);
    }

    private static void RenderPortDevice(
        string? model,
        bool? managed,
        bool? poe,
        List<Port>? ports)
    {
        if (managed.HasValue)
            AnsiConsole.MarkupLine($"  Managed: {(managed.Value ? "yes" : "no")}");
        if (poe.HasValue)
            AnsiConsole.MarkupLine($"  PoE:     {(poe.Value ? "yes" : "no")}");

        if (ports is { Count: > 0 })
        {
            AnsiConsole.MarkupLine("  Ports:");
            foreach (var p in ports)
                AnsiConsole.MarkupLine($"    {p.Count ?? 0}x {p.Type} @ {p.Speed ?? 0}G");
        }
    }
}
