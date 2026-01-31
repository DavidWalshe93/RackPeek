namespace RackPeek.Domain.Resources.Hardware.Laptops;

public class DeleteLaptopUseCase(IHardwareRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string name)
    {
        var hardware = await repository.GetByNameAsync(name);
        if (hardware == null)
            throw new InvalidOperationException($"Laptop '{name}' not found.");

        await repository.DeleteAsync(name);
    }
}