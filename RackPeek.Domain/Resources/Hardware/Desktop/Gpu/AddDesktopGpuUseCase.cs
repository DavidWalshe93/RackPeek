using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class AddDesktopGpuUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string desktopName, Gpu gpu)
    {
        var desktop = await repository.GetByNameAsync(desktopName) as Models.Desktop
                      ?? throw new InvalidOperationException($"Desktop '{desktopName}' not found.");

        desktop.Gpus ??= new List<Gpu>();
        desktop.Gpus.Add(gpu);

        await repository.UpdateAsync(desktop);
    }
}