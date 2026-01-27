using Tests.EndToEnd.Infra;
using Xunit.Abstractions;

namespace Tests.EndToEnd;

[Collection("Yaml CLI tests")]
public class ServiceYamlE2ETests(TempYamlCliFixture fs, ITestOutputHelper outputHelper)
    : IClassFixture<TempYamlCliFixture>
{
    private async Task<(string, string)> ExecuteAsync(params string[] args)
    {
        outputHelper.WriteLine($"rpk {string.Join(" ", args)}");

        var inputArgs = args.ToArray();
        var output = await YamlCliTestHost.RunAsync(
            inputArgs,
            fs.Root,
            outputHelper,
            "config.yaml"
        );

        outputHelper.WriteLine(output);

        var yaml = await File.ReadAllTextAsync(Path.Combine(fs.Root, "config.yaml"));
        return (output, yaml);
    }

    [Fact]
    public async Task systems_cli_workflow_test()
    {
        await File.WriteAllTextAsync(Path.Combine(fs.Root, "config.yaml"), "");

        // Add system
        var (output, yaml) = await ExecuteAsync("services", "add", "immich");
        Assert.Equal("Service 'immich' added.\n", output);
        Assert.Equal("""
                     resources:
                     - kind: Service
                       network: 
                       runsOn: 
                       name: immich
                       tags: 

                     """, yaml);

        // Update system
        (output, yaml) = await ExecuteAsync(
            "services", "set", "immich",
            "--ip", "192.168.10.14",
            "--port", "80",
            "--protocol", "TCP",
            "--url", "http://timmoth.lan:80",
            "--runs-on", "vm01"
        );

        Assert.Equal("Service 'immich' updated.\n", output);

        Assert.Equal("""
                     resources:
                     - kind: Service
                       network:
                         ip: 192.168.10.14
                         port: 80
                         protocol: TCP
                         url: http://timmoth.lan:80
                       runsOn: vm01
                       name: immich
                       tags: 

                     """, yaml);

        // Get system by name
        (output, yaml) = await ExecuteAsync("services", "get", "immich");
        Assert.Equal(
            "immich  Ip: 192.168.10.14, Port: 80, Protocol: TCP, Url: http://timmoth.lan:80, \nRunsOn: vm01\n",
            output);

        // List systems
        (output, yaml) = await ExecuteAsync("services", "list");
        Assert.Equal("""
                     ╭────────┬───────────────┬──────┬──────────┬───────────────────────┬─────────╮
                     │ Name   │ Ip            │ Port │ Protocol │ Url                   │ Runs On │
                     ├────────┼───────────────┼──────┼──────────┼───────────────────────┼─────────┤
                     │ immich │ 192.168.10.14 │ 80   │ TCP      │ http://timmoth.lan:80 │ vm01    │
                     ╰────────┴───────────────┴──────┴──────────┴───────────────────────┴─────────╯
                     
                     """, output);

        // Report systems
        (output, yaml) = await ExecuteAsync("services", "summary");
        Assert.Equal("""
                     ╭────────┬───────────────┬──────┬──────────┬───────────────────────┬─────────╮
                     │ Name   │ Ip            │ Port │ Protocol │ Url                   │ Runs On │
                     ├────────┼───────────────┼──────┼──────────┼───────────────────────┼─────────┤
                     │ immich │ 192.168.10.14 │ 80   │ TCP      │ http://timmoth.lan:80 │ vm01    │
                     ╰────────┴───────────────┴──────┴──────────┴───────────────────────┴─────────╯
                     
                     """, output);

        // Delete system
        (output, yaml) = await ExecuteAsync("services", "del", "immich");
        Assert.Equal("""
                     Service 'immich' deleted.

                     """, output);

        // Ensure list is empty
        (output, yaml) = await ExecuteAsync("services", "list");
        Assert.Equal("""
                     No Services found.

                     """, output);
    }
}