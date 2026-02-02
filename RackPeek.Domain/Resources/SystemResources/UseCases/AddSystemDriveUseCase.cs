using System.ComponentModel.DataAnnotations;
using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.SystemResources;

namespace RackPeek.Domain.Resources.SystemResources.UseCases;

public class AddSystemDriveUseCase(ISystemRepository repository) : IUseCase
{
    public async Task ExecuteAsync(string systemName, string type, int size)
    {
        ThrowIfInvalid.ResourceName(systemName);
        ThrowIfInvalid.ResourceName(type);

        if (size < 0)
            throw new ValidationException("Drive size must be non‑negative.");

        var system = await repository.GetByNameAsync(systemName)
                     ?? throw new NotFoundException($"System '{systemName}' not found.");

        system.Drives ??= new List<Drive>();

        system.Drives.Add(new Drive
        {
            Type = type,
            Size = size
        });

        await repository.UpdateAsync(system);
    }
}