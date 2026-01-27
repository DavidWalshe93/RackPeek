using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class AddDesktopDriveUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string desktopName, Drive drive)
    {
        var desktop = await repository.GetByNameAsync(desktopName) as Models.Desktop
                      ?? throw new InvalidOperationException($"Desktop '{desktopName}' not found.");

        desktop.Drives ??= new List<Drive>();
        desktop.Drives.Add(drive);

        await repository.UpdateAsync(desktop);
    }
}