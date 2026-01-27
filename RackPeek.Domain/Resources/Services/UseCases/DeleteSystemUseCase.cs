using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Services.UseCases;

public class DeleteServiceUseCase(IServiceRepository repository)
{
    public async Task ExecuteAsync(string name)
    {
        if (await repository.GetByNameAsync(name) is not Service)
            throw new InvalidOperationException($"Service '{name}' not found.");

        await repository.DeleteAsync(name);
    }
}