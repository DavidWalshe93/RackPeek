using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Gpus;

public class UpdateLaptopGpuUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string LaptopName, int index, Gpu updated)
    {
        var Laptop = await repository.GetByNameAsync(LaptopName) as Laptop
                     ?? throw new InvalidOperationException($"Laptop '{LaptopName}' not found.");

        if (Laptop.Gpus == null || index < 0 || index >= Laptop.Gpus.Count)
            throw new InvalidOperationException($"GPU index {index} not found on Laptop '{LaptopName}'.");

        Laptop.Gpus[index] = updated;

        await repository.UpdateAsync(Laptop);
    }
}