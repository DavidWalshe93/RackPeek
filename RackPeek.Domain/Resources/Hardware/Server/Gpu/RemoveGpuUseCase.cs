namespace RackPeek.Domain.Resources.Hardware.Server.Gpu;

public class RemoveGpuUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string serverName, int index)
    {
        var hardware = await repository.GetByNameAsync(serverName);

        if (hardware is not Models.Server server) 
            return;

        server.Gpus ??= [];

        if (index < 0 || index >= server.Gpus.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "GPU index out of range.");

        server.Gpus.RemoveAt(index);

        await repository.UpdateAsync(server);
    }
}