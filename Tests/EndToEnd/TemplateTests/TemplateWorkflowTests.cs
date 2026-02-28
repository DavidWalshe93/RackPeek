using Tests.EndToEnd.Infra;
using Xunit.Abstractions;

namespace Tests.EndToEnd.TemplateTests;

[Collection("Yaml CLI tests")]
public class TemplateWorkflowTests(TempYamlCliFixture fs, ITestOutputHelper outputHelper)
    : IClassFixture<TempYamlCliFixture>
{
    private async Task<(string Output, string Yaml)> ExecuteAsync(params string[] args)
    {
        outputHelper.WriteLine($"rpk {string.Join(" ", args)}");

        var output = await YamlCliTestHost.RunAsync(
            args,
            fs.Root,
            outputHelper,
            "config.yaml"
        );

        outputHelper.WriteLine(output);

        var yaml = await File.ReadAllTextAsync(Path.Combine(fs.Root, "config.yaml"));
        return (output, yaml);
    }

    [Fact]
    public async Task template_list__returns_bundled_templates()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, _) = await ExecuteAsync("templates", "list");

        // Assert — should contain at least some known templates
        Assert.Contains("Switch", output);
        Assert.Contains("UniFi-USW-Enterprise-24", output);
        Assert.Contains("Router", output);
        Assert.Contains("Firewall", output);
    }

    [Fact]
    public async Task template_list__filter_by_kind__returns_only_matching()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, _) = await ExecuteAsync("templates", "list", "--kind", "Switch");

        // Assert
        Assert.Contains("Switch", output);
        Assert.DoesNotContain("Router", output);
        Assert.DoesNotContain("Firewall", output);
    }

    [Fact]
    public async Task template_show__existing_template__shows_details()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, _) = await ExecuteAsync("templates", "show", "Switch/UniFi-USW-Enterprise-24");

        // Assert
        Assert.Contains("Switch/UniFi-USW-Enterprise-24", output);
        Assert.Contains("Switch", output);
        Assert.Contains("UniFi-USW-Enterprise-24", output);
    }

    [Fact]
    public async Task template_show__nonexistent_template__returns_error()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, _) = await ExecuteAsync("templates", "show", "Switch/DoesNotExist");

        // Assert
        Assert.Contains("not found", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task switch_add_with_template__creates_prefilled_switch()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, yaml) = await ExecuteAsync(
            "switches", "add", "core-switch", "--template", "UniFi-USW-Enterprise-24");

        // Assert
        Assert.Equal("Switch 'core-switch' added.\n", output);
        Assert.Contains("name: core-switch", yaml);
        Assert.Contains("model: UniFi-USW-Enterprise-24", yaml);
        Assert.Contains("managed: true", yaml);
        Assert.Contains("poe: true", yaml);
    }

    [Fact]
    public async Task router_add_with_template__creates_prefilled_router()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, yaml) = await ExecuteAsync(
            "routers", "add", "edge-router", "--template", "Ubiquiti-ER-4");

        // Assert
        Assert.Equal("Router 'edge-router' added.\n", output);
        Assert.Contains("name: edge-router", yaml);
        Assert.Contains("model: Ubiquiti-ER-4", yaml);
        Assert.Contains("managed: true", yaml);
    }

    [Fact]
    public async Task firewall_add_with_template__creates_prefilled_firewall()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, yaml) = await ExecuteAsync(
            "firewalls", "add", "main-fw", "--template", "Netgate-6100");

        // Assert
        Assert.Equal("Firewall 'main-fw' added.\n", output);
        Assert.Contains("name: main-fw", yaml);
        Assert.Contains("model: Netgate-6100", yaml);
    }

    [Fact]
    public async Task switch_add_with_template__describe_shows_ports()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act — create with template then describe
        await ExecuteAsync("switches", "add", "test-sw", "--template", "UniFi-USW-16-PoE");
        var (output, _) = await ExecuteAsync("switches", "describe", "test-sw");

        // Assert — describe output should contain the template's port data
        Assert.Contains("test-sw", output);
        Assert.Contains("UniFi-USW-16-PoE", output);
    }

    [Fact]
    public async Task switch_add_without_template__creates_blank()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, yaml) = await ExecuteAsync("switches", "add", "blank-switch");

        // Assert
        Assert.Equal("Switch 'blank-switch' added.\n", output);
        Assert.Contains("name: blank-switch", yaml);
        Assert.DoesNotContain("model:", yaml);
    }

    [Fact]
    public async Task switch_add_with_invalid_template__returns_error()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Act
        var (output, _) = await ExecuteAsync(
            "switches", "add", "bad-switch", "--template", "NonExistentModel");

        // Assert
        Assert.Contains("not found", output, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task switch_add_with_template__duplicate_name__returns_error()
    {
        // Arrange
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");
        await ExecuteAsync("switches", "add", "dupe-switch");

        // Act
        var (output, _) = await ExecuteAsync(
            "switches", "add", "dupe-switch", "--template", "UniFi-USW-Enterprise-24");

        // Assert
        Assert.Contains("already exists", output, StringComparison.OrdinalIgnoreCase);
    }
}
