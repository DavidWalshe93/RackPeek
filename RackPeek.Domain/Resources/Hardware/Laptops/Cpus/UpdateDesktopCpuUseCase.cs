using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Cpus;

public class UpdateLaptopCpuUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string LaptopName, int index, Cpu updated)
    {
        var Laptop = await repository.GetByNameAsync(LaptopName) as Laptop
                     ?? throw new InvalidOperationException($"Laptop '{LaptopName}' not found.");

        if (Laptop.Cpus == null || index < 0 || index >= Laptop.Cpus.Count)
            throw new InvalidOperationException($"CPU index {index} not found on Laptop '{LaptopName}'.");

        Laptop.Cpus[index] = updated;

        await repository.UpdateAsync(Laptop);
    }
}