namespace RackPeek.Domain.Resources.Hardware.Desktops;

public class DeleteDesktopUseCase
{
    private readonly IHardwareRepository _repository;

    public DeleteDesktopUseCase(IHardwareRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(string name)
    {
        var hardware = await _repository.GetByNameAsync(name);
        if (hardware == null)
            throw new InvalidOperationException($"Desktop '{name}' not found.");

        await _repository.DeleteAsync(name);
    }
}