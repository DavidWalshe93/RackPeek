using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public record DesktopDescription(
    string Name,
    string? Model,
    int CpuCount,
    string? RamSummary,
    int DriveCount,
    int NicCount,
    int GpuCount
);

public class DescribeDesktopUseCase(IHardwareRepository repository)
{
    public async Task<DesktopDescription?> ExecuteAsync(string name)
    {
        var desktop = await repository.GetByNameAsync(name) as Models.Desktop;
        if (desktop == null)
            return null;

        var ramSummary = desktop.Ram == null
            ? "None"
            : $"{desktop.Ram.Size} GB @ {desktop.Ram.Mts} MT/s";

        return new DesktopDescription(
            desktop.Name,
            desktop.Model,
            desktop.Cpus?.Count ?? 0,
            ramSummary,
            desktop.Drives?.Count ?? 0,
            desktop.Nics?.Count ?? 0,
            desktop.Gpus?.Count ?? 0
        );
    }
}