using RackPeek.Domain;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace RackPeek;

public class ConsoleEmulator : IConsoleEmulator
{
    public CommandApp App { get; set; }
    public ConsoleEmulator(IServiceCollection services)
    {
        var registrar = new TypeRegistrar(services);
        App = new CommandApp(registrar);
        CliBootstrap.BuildApp(App);
    }

    public async Task<string> Execute(string input)
    {
        // create fresh console every run
        var testConsole = new TestConsole();
        testConsole.Width(120);

        AnsiConsole.Console = testConsole;
        App.Configure(c => c.Settings.Console = testConsole);

        await App.RunAsync(input.Split(" ", StringSplitOptions.RemoveEmptyEntries));

        return testConsole.Output;
    }

}

public sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
    public void Register(Type service, Type implementation)
    {
        services.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        services.AddSingleton(service, _ => factory());
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(services.BuildServiceProvider());
    }
}

public sealed class TypeResolver(ServiceProvider provider) : ITypeResolver, IDisposable
{
    public void Dispose()
    {
        provider.Dispose();
    }

    public object? Resolve(Type? type)
    {
        return type == null ? null : provider.GetService(type);
    }
}