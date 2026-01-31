using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Firewalls;

public class GetFirewallsUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<IReadOnlyList<Firewall>> ExecuteAsync()
    {
        var hardware = await repository.GetAllAsync();
        return hardware.OfType<Firewall>().ToList();
    }
}