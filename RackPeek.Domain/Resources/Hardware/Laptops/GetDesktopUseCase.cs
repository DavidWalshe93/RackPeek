using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops;

public class GetLaptopUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<Laptop?> ExecuteAsync(string name)
    {
        var hardware = await repository.GetByNameAsync(name);
        return hardware as Laptop;
    }
}