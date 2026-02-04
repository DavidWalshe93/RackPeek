using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Switches.Ports;

public class RemoveSwitchPortUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string name, int index)
    {
        name = Normalize.HardwareName(name);
        ThrowIfInvalid.ResourceName(name);

        var Switch = await repository.GetByNameAsync(name) as Switch
                      ?? throw new NotFoundException($"Switch '{name}' not found.");

        if (Switch.Ports == null || index < 0 || index >= Switch.Ports.Count)
            throw new NotFoundException($"Port index {index} not found on Switch '{name}'.");

        Switch.Ports.RemoveAt(index);

        await repository.UpdateAsync(Switch);
    }
}