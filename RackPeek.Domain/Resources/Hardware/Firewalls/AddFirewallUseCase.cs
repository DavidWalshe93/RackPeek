using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Firewalls;

public class AddFirewallUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string name)
    {
        // basic guard rails
        var existing = await repository.GetByNameAsync(name);
        if (existing != null)
            throw new InvalidOperationException($"Firewall '{name}' already exists.");

        var FirewallResource = new Firewall
        {
            Name = name
        };

        await repository.AddAsync(FirewallResource);
    }
}