using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class UpdateDesktopGpuUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string desktopName, int index, Gpu updated)
    {
        var desktop = await repository.GetByNameAsync(desktopName) as Models.Desktop
                      ?? throw new InvalidOperationException($"Desktop '{desktopName}' not found.");

        if (desktop.Gpus == null || index < 0 || index >= desktop.Gpus.Count)
            throw new InvalidOperationException($"GPU index {index} not found on desktop '{desktopName}'.");

        desktop.Gpus[index] = updated;

        await repository.UpdateAsync(desktop);
    }
}