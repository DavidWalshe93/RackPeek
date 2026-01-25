namespace RackPeek.Domain.Resources.Hardware.Server.Drive;

public class UpdateDriveUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string serverName, int index, string type, int size)
    {
        var hardware = await repository.GetByNameAsync(serverName);
        if (hardware is not Models.Server server)
        {
            return;
        }

        server.Drives ??= [];
        if (index < 0 || index >= server.Drives.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Drive index out of range.");
        var drive = server.Drives[index];
        drive.Type = type;
        drive.Size = size;
        await repository.UpdateAsync(server);
    }
}