using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktops.Drives;

public class AddDesktopDriveUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string name, Drive drive)
    {
        // ToDo pass in properties as inputs, construct the entity in the usecase, ensure optional inputs are nullable
        // ToDo validate / normalize all inputs
        
        name = Normalize.HardwareName(name);
        ThrowIfInvalid.ResourceName(name);

        var desktop = await repository.GetByNameAsync(name) as Desktop
                      ?? throw new NotFoundException($"Desktop '{name}' not found.");

        desktop.Drives ??= new List<Drive>();
        desktop.Drives.Add(drive);

        await repository.UpdateAsync(desktop);
    }
}