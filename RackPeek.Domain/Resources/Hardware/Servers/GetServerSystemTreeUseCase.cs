using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Hardware.Servers;

public class GetServerSystemTreeUseCase(
    IHardwareRepository hardwareRepository,
    ISystemRepository systemRepository)
{
    public async Task<HardwareDependencyTree?> ExecuteAsync(string hardwareName)
    {
        var server = await hardwareRepository.GetByNameAsync(hardwareName) as Server;
        if (server is null) return null;

        var systems = await systemRepository.GetByPhysicalHostAsync(hardwareName);

        return new HardwareDependencyTree(server, systems);
    }
}

public sealed class HardwareDependencyTree(Server hardware, IReadOnlyList<SystemResource> systems)
{
    public Server Hardware { get; } = hardware;
    public IReadOnlyList<SystemResource> Systems { get; } = systems;
}