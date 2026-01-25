namespace RackPeek.Domain.Resources.Hardware.Server.Drive;

public class RemoveDriveUseCase(IHardwareRepository repository)
{
    public async Task ExecuteAsync(string serverName, int index)
    {
        var hardware = await repository.GetByNameAsync(serverName);
        if (hardware is not Models.Server server) return;
        server.Drives ??= [];
        if (index < 0 || index >= server.Drives.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Drive index out of range.");
        server.Drives.RemoveAt(index);
        await repository.UpdateAsync(server);
    }
}