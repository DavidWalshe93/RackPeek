namespace RackPeek.Domain.Resources.Hardware.Server.Drive;

public class AddDrivesUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(
        string serverName,
        string type,
        int size)
    {
        var hardware = await repository.GetByNameAsync(serverName);

        if (hardware is not Models.Server server)
        {
            return;
        }

        server.Drives ??= [];

        server.Drives.Add(new Models.Drive
        {
            Type = type,
            Size = size
        });
        
        await repository.UpdateAsync(server);
    }
}