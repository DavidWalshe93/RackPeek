using RackPeek.Domain.Ansible;
using RackPeek.Domain.Helpers;
using RackPeek.Domain.Persistence;
using RackPeek.Domain.Resources.SubResources;

namespace RackPeek.Domain.Resources.Desktops;

public class AnsibleInventoryGeneratorUseCase(IResourceCollection repository) : IUseCase
{
    public async Task<InventoryResult?> ExecuteAsync(InventoryOptions options)
    {
        var resources = await repository.GetAllOfTypeAsync<Resource>();
        return resources.ToAnsibleInventoryIni(options);
    }
}