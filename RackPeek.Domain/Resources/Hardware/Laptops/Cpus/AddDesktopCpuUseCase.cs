using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Cpus;

public class AddLaptopCpuUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string LaptopName, Cpu cpu)
    {
        var Laptop = await repository.GetByNameAsync(LaptopName) as Laptop
                      ?? throw new InvalidOperationException($"Laptop '{LaptopName}' not found.");

        Laptop.Cpus ??= new List<Cpu>();
        Laptop.Cpus.Add(cpu);

        await repository.UpdateAsync(Laptop);
    }
}