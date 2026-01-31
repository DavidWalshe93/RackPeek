using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Drives;

public class AddLaptopDriveUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string LaptopName, Drive drive)
    {
        var Laptop = await repository.GetByNameAsync(LaptopName) as Laptop
                     ?? throw new InvalidOperationException($"Laptop '{LaptopName}' not found.");

        Laptop.Drives ??= new List<Drive>();
        Laptop.Drives.Add(drive);

        await repository.UpdateAsync(Laptop);
    }
}