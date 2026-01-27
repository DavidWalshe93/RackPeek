using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class GetDesktopUseCase(IHardwareRepository repository)
{
    public async Task<Models.Desktop?> ExecuteAsync(string name)
    {
        var hardware = await repository.GetByNameAsync(name);
        return hardware as Models.Desktop;
    }
}