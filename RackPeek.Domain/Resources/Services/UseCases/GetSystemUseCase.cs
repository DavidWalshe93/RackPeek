using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Services.UseCases;

public class GetServiceUseCase(IServiceRepository repository)
{
    public async Task<Service?> ExecuteAsync(string name)
    {
        return await repository.GetByNameAsync(name);
    }
}