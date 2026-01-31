using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Gpus;

public class AddLaptopGpuUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string LaptopName, Gpu gpu)
    {
        var Laptop = await repository.GetByNameAsync(LaptopName) as Laptop
                     ?? throw new InvalidOperationException($"Laptop '{LaptopName}' not found.");

        Laptop.Gpus ??= new List<Gpu>();
        Laptop.Gpus.Add(gpu);

        await repository.UpdateAsync(Laptop);
    }
}