using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktops;

public class UpdateDesktopUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(
        string name,
        string? model = null,
        int? ramGb = null,
        int? ramMts = null
    )
    {
        // ToDo validate / normalize all inputs
        
        name = Normalize.HardwareName(name);
        ThrowIfInvalid.ResourceName(name);

        var desktop = await repository.GetByNameAsync(name) as Desktop;
        if (desktop == null)
            throw new NotFoundException($"Desktop '{name}' not found.");

        if (!string.IsNullOrWhiteSpace(model))
            desktop.Model = model;
        
        // ---- RAM ----
        if (ramGb.HasValue)
        {
            ThrowIfInvalid.RamGb(ramGb);
            desktop.Ram ??= new Ram();
            desktop.Ram.Size = ramGb.Value;
        }
        
        if (ramMts.HasValue)
        {
            desktop.Ram ??= new Ram();
            desktop.Ram.Mts = ramMts.Value;
        }

        await repository.UpdateAsync(desktop);
    }
}