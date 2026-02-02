using System.ComponentModel.DataAnnotations;
using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.SystemResources.UseCases;

public class UpdateSystemDriveUseCase(ISystemRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string systemName, int index, string type, int size)
    {
        ThrowIfInvalid.ResourceName(systemName);
        ThrowIfInvalid.ResourceName(type);

        if (size < 0)
            throw new ValidationException("Drive size must be non‑negative.");

        var system = await repository.GetByNameAsync(systemName)
                     ?? throw new NotFoundException($"System '{systemName}' not found.");

        if (system.Drives == null || index < 0 || index >= system.Drives.Count)
            throw new NotFoundException($"Drive index {index} not found on system '{systemName}'.");

        var drive = system.Drives[index];

        drive.Type = type;
        drive.Size = size;

        await repository.UpdateAsync(system);
    }
}