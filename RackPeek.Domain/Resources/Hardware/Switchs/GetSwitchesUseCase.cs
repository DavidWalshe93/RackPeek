using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Switchs;

public class GetSwitchesUseCase(IHardwareRepository repository)
{
    public async Task<IReadOnlyList<Switch>> ExecuteAsync()
    {
        var hardware = await repository.GetAllAsync();
        return hardware.OfType<Switch>().ToList();
    }
}