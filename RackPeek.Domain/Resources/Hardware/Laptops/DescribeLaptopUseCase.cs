using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops;

public class DescribeLaptopUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<LaptopDescription?> ExecuteAsync(string name)
    {
        var Laptop = await repository.GetByNameAsync(name) as Laptop;
        if (Laptop == null)
            return null;

        var ramSummary = Laptop.Ram == null
            ? "None"
            : $"{Laptop.Ram.Size} GB @ {Laptop.Ram.Mts} MT/s";

        return new LaptopDescription(
            Laptop.Name,
            Laptop.Cpus?.Count ?? 0,
            ramSummary,
            Laptop.Drives?.Count ?? 0,
            Laptop.Gpus?.Count ?? 0
        );
    }
}

public record LaptopDescription(
    string Name,
    int CpuCount,
    string? RamSummary,
    int DriveCount,
    int GpuCount
);