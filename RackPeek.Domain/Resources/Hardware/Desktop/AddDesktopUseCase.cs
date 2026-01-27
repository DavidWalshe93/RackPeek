using RackPeek.Domain.Resources.Hardware.Models;

namespace RackPeek.Domain.Resources.Hardware.Desktop;

public class AddDesktopUseCase
{
    private readonly IHardwareRepository _repository;

    public AddDesktopUseCase(IHardwareRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(string name)
    {
        var existing = await _repository.GetByNameAsync(name);
        if (existing != null)
            throw new InvalidOperationException($"Desktop '{name}' already exists.");

        var desktop = new Models.Desktop
        {
            Name = name,
            Cpus = new List<Cpu>(),
            Drives = new List<Drive>(),
            Nics = new List<Nic>(),
            Gpus = new List<Gpu>(),
            Ram = null
        };

        await _repository.AddAsync(desktop);
    }
}