using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Services.UseCases;

public record ServiceReport(
    IReadOnlyList<ServiceReportRow> Services
);

public record ServiceReportRow(
    string Name,
    string? Ip,
    int? Port,
    string? Protocol,
    string? Url,
    string? RunsOn
);

public class ServiceReportUseCase(IServiceRepository repository)
{
    public async Task<ServiceReport> ExecuteAsync()
    {
        var services = await repository.GetAllAsync();

        var rows = services.Select(s =>
        {
            return new ServiceReportRow(
                s.Name,
                s.Network?.Ip,
                s.Network?.Port,
                s.Network?.Protocol,
                s.Network?.Url,
                s.RunsOn
            );
        }).ToList();

        return new ServiceReport(rows);
    }
}