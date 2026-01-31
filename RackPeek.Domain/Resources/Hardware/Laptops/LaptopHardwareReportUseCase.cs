using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops;

public class LaptopHardwareReportUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<LaptopHardwareReport> ExecuteAsync()
    {
        var hardware = await repository.GetAllAsync();
        var Laptops = hardware.OfType<Laptop>();

        var rows = Laptops.Select(Laptop =>
        {
            var totalCores = Laptop.Cpus?.Sum(c => c.Cores) ?? 0;
            var totalThreads = Laptop.Cpus?.Sum(c => c.Threads) ?? 0;

            var cpuSummary = Laptop.Cpus == null
                ? "Unknown"
                : string.Join(", ",
                    Laptop.Cpus
                        .GroupBy(c => c.Model)
                        .Select(g => $"{g.Count()}× {g.Key}"));

            var ramGb = Laptop.Ram?.Size ?? 0;

            var totalStorage = Laptop.Drives?.Sum(d => d.Size) ?? 0;
            var ssdStorage = Laptop.Drives?
                .Where(d => d.Type == "ssd")
                .Sum(d => d.Size) ?? 0;
            var hddStorage = Laptop.Drives?
                .Where(d => d.Type == "hdd")
                .Sum(d => d.Size) ?? 0;

            var gpuSummary = Laptop.Gpus == null
                ? "None"
                : string.Join(", ",
                    Laptop.Gpus
                        .GroupBy(g => g.Model)
                        .Select(g => $"{g.Count()}× {g.Key}"));

            return new LaptopHardwareRow(
                Laptop.Name,
                cpuSummary,
                totalCores,
                totalThreads,
                ramGb,
                totalStorage,
                ssdStorage,
                hddStorage,
                gpuSummary
            );
        }).ToList();

        return new LaptopHardwareReport(rows);
    }
}

public record LaptopHardwareReport(
    IReadOnlyList<LaptopHardwareRow> Laptops
);

public record LaptopHardwareRow(
    string Name,
    string CpuSummary,
    int TotalCores,
    int TotalThreads,
    int RamGb,
    int TotalStorageGb,
    int SsdStorageGb,
    int HddStorageGb,
    string GpuSummary
);