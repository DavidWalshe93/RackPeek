namespace RackPeek.Domain.Resources.Hardware.Server.Gpu;

public class UpdateGpuUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(
        string serverName,
        int index,
        string model,
        int vram)
    {
        var hardware = await repository.GetByNameAsync(serverName);

        if (hardware is not Models.Server server) 
            return;

        server.Gpus ??= [];

        if (index < 0 || index >= server.Gpus.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "GPU index out of range.");

        var gpu = server.Gpus[index];
        gpu.Model = model;
        gpu.Vram = vram;

        await repository.UpdateAsync(server);
    }
}