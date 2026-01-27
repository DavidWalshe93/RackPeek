using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Services.UseCases;

public record ServiceDescription(
    string Name,
    string? Ip,
    int? Port,
    string? Protocol,
    string? Url,
    string? RunsOn
);

public class DescribeServiceUseCase(IServiceRepository repository)
{
    public async Task<ServiceDescription?> ExecuteAsync(string name)
    {
        var service = await repository.GetByNameAsync(name);
        if (service is null)
            return null;

        return new ServiceDescription(
            service.Name,
            service.Network?.Ip,
            service.Network?.Port,
            service.Network?.Protocol,
            service.Network?.Url,
            service.RunsOn
        );
    }
}