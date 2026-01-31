using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Routers;

public class GetRouterUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<Router?> ExecuteAsync(string name)
    {
        var hardware = await repository.GetByNameAsync(name);
        return hardware as Router;
    }
}