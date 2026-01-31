using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops.Drives;

public class UpdateLaptopDriveUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string LaptopName, int index, Drive updated)
    {
        var Laptop = await repository.GetByNameAsync(LaptopName) as Laptop
                      ?? throw new InvalidOperationException($"Laptop '{LaptopName}' not found.");

        if (Laptop.Drives == null || index < 0 || index >= Laptop.Drives.Count)
            throw new InvalidOperationException($"Drive index {index} not found on Laptop '{LaptopName}'.");

        Laptop.Drives[index] = updated;

        await repository.UpdateAsync(Laptop);
    }
}