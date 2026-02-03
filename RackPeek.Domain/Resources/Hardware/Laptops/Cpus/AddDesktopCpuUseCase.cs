using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Cpus;

public class AddLaptopCpuUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string name, Cpu cpu)
    {
        // ToDo pass in properties as inputs, construct the entity in the usecase, ensure optional inputs are nullable
        // ToDo validate / normalize all inputs
        
        name = Normalize.HardwareName(name);
        ThrowIfInvalid.ResourceName(name);
        var laptop = await repository.GetByNameAsync(name) as Laptop
                     ?? throw new InvalidOperationException($"Laptop '{name}' not found.");

        laptop.Cpus ??= new List<Cpu>();
        laptop.Cpus.Add(cpu);

        await repository.UpdateAsync(laptop);
    }
}