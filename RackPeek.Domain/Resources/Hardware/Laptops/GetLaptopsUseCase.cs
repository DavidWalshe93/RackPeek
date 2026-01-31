using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Laptops;

public class GetLaptopsUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task<IReadOnlyList<Laptop>> ExecuteAsync()
    {
        var hardware = await repository.GetAllAsync();
        return hardware.OfType<Laptop>().ToList();
    }
}