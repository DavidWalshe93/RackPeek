
namespace RackPeek.Domain.Resources.Hardware.Server.Gpu;

public class AddGpuUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(
        string serverName,
        string model,
        int vram)
    {
        var hardware = await repository.GetByNameAsync(serverName);

        if (hardware is not Models.Server server) 
            return;

        server.Gpus ??= [];

        server.Gpus.Add(new Models.Gpu
        {
            Model = model,
            Vram = vram
        });

        await repository.UpdateAsync(server);
    }
}