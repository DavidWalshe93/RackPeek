using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Routers;

public class GetRoutersUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<IReadOnlyList<Router>> ExecuteAsync()
    {
        var hardware = await repository.GetAllAsync();
        return hardware.OfType<Router>().ToList();
    }
}