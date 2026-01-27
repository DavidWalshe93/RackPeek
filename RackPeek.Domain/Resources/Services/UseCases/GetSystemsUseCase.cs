using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Services.UseCases;

public class GetServicesUseCase(IServiceRepository repository)
{
    public async Task<IReadOnlyList<Service>> ExecuteAsync()
    {
        return await repository.GetAllAsync();
    }
}