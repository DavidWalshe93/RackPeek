using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktops.Nics;

public class AddDesktopNicUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string name, Nic nic)
    {
        // ToDo pass in properties as inputs, construct the entity in the usecase, ensure optional inputs are nullable
        // ToDo validate / normalize all inputs
        
        name = Normalize.HardwareName(name);
        ThrowIfInvalid.ResourceName(name);

        var desktop = await repository.GetByNameAsync(name) as Desktop
                      ?? throw new NotFoundException($"Desktop '{name}' not found.");

        desktop.Nics ??= new List<Nic>();
        desktop.Nics.Add(nic);

        await repository.UpdateAsync(desktop);
    }
}