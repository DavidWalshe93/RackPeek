using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;

public class DescribeDesktopUseCaseTests
{
    [Fact]
    public async Task Returns_Description()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("desk1").Returns(new Desktop
        {
            Name = "desk1",
            Model = "Optiplex",
            Cpus = new() { new Cpu() },
            Ram = new Ram { Size = 16, Mts = 2666 },
            Drives = new() { new Drive() },
            Nics = new() { new Nic() },
            Gpus = new() { new Gpu() }
        });

        var useCase = new DescribeDesktopUseCase(repo);

        var result = await useCase.ExecuteAsync("desk1");

        Assert.NotNull(result);
        Assert.Equal("desk1", result!.Name);
        Assert.Equal(1, result.CpuCount);
        Assert.Equal(1, result.DriveCount);
        Assert.Equal(1, result.NicCount);
        Assert.Equal(1, result.GpuCount);
    }

    [Fact]
    public async Task Returns_Null_If_Not_Found()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("desk1").Returns((Hardware?)null);

        var useCase = new DescribeDesktopUseCase(repo);

        var result = await useCase.ExecuteAsync("desk1");

        Assert.Null(result);
    }
}