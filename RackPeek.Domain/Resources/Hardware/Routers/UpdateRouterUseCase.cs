using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Routers;

public class UpdateRouterUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(
        string name,
        string? model = null,
        bool? managed = null,
        bool? poe = null
    )
    {
        var RouterResource = await repository.GetByNameAsync(name) as Router;
        if (RouterResource == null)
            throw new InvalidOperationException($"Router '{name}' not found.");

        if (!string.IsNullOrWhiteSpace(model))
            RouterResource.Model = model;

        if (managed.HasValue)
            RouterResource.Managed = managed.Value;

        if (poe.HasValue)
            RouterResource.Poe = poe.Value;

        await repository.UpdateAsync(RouterResource);
    }
}