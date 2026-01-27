using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class AddDesktopCpuUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string desktopName, Cpu cpu)
    {
        var desktop = await repository.GetByNameAsync(desktopName) as Models.Desktop
                      ?? throw new InvalidOperationException($"Desktop '{desktopName}' not found.");

        desktop.Cpus ??= new List<Cpu>();
        desktop.Cpus.Add(cpu);

        await repository.UpdateAsync(desktop);
    }
}