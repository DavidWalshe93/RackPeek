using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Services;
using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.Hardware.Desktops;

public class GetDesktopSystemTreeUseCase(
    IHardwareRepository hardwareRepository,
    ISystemRepository systemRepository,
    IServiceRepository serviceRepository) : IUseCase
{
    public async Task<HardwareDependencyTree?> ExecuteAsync(string hardwareName)
    {
        if (string.IsNullOrWhiteSpace(hardwareName))
            return null;

        var desktop = await hardwareRepository.GetByNameAsync(hardwareName) as Desktop;
        if (desktop is null)
            return null;

        return await BuildDependencyTreeAsync(desktop);
    }

    private async Task<HardwareDependencyTree> BuildDependencyTreeAsync(Desktop desktop)
    {
        var systems = await systemRepository.GetByPhysicalHostAsync(desktop.Name);

        var systemTrees = new List<SystemDependencyTree>();
        foreach (var system in systems)
            systemTrees.Add(await BuildSystemDependencyTreeAsync(system));

        return new HardwareDependencyTree(desktop, systemTrees);
    }

    private async Task<SystemDependencyTree> BuildSystemDependencyTreeAsync(SystemResource system)
    {
        var services = await serviceRepository.GetBySystemHostAsync(system.Name);
        return new SystemDependencyTree(system, services);
    }
}

public sealed class HardwareDependencyTree(Desktop hardware, IEnumerable<SystemDependencyTree> systems)
{
    public Desktop Hardware { get; } = hardware;
    public IEnumerable<SystemDependencyTree> Systems { get; } = systems;
}

public sealed class SystemDependencyTree(SystemResource system, IEnumerable<Service> services)
{
    public SystemResource System { get; } = system;
    public IEnumerable<Service> Services { get; } = services;
}