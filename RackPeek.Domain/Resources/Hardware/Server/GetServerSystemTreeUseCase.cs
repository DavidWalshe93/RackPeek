using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Hardware.Server;

public class GetServerSystemTreeUseCase(
    IHardwareRepository hardwareRepository,
    ISystemRepository systemRepository)
{
    public async Task<HardwareDependencyTree?> ExecuteAsync(string hardwareName)
    {
        var server = await hardwareRepository.GetByNameAsync(hardwareName) as Models.Server;
        if (server is null) return null;

        var systems = await systemRepository.GetByPhysicalHostAsync(hardwareName);

        return new HardwareDependencyTree(server, systems);
    }
}

public sealed class HardwareDependencyTree(Models.Server hardware, IReadOnlyList<SystemResource> systems)
{
    public Models.Server Hardware { get; } = hardware;
    public IReadOnlyList<SystemResource> Systems { get; } = systems;
}