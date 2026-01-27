using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class GetDesktopsUseCase(IHardwareRepository repository)
{
    public async Task<IReadOnlyList<Models.Desktop>> ExecuteAsync()
    {
        var hardware = await repository.GetAllAsync();
        return hardware.OfType<Models.Desktop>().ToList();
    }
}