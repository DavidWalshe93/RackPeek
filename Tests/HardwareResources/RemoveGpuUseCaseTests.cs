using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Servers.Gpus;

namespace Tests.HardwareResources;

public class RemoveGpuUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Removes_gpu_when_index_is_valid()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Gpus = new List<Gpu>
            {
                new() { Model = "RTX 4090", Vram = 24 },
                new() { Model = "RTX 3080", Vram = 10 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new RemoveGpuUseCase(repo);

        // Act
        await sut.ExecuteAsync("node01", 0);

        // Assert
        Assert.Single(server.Gpus);
        Assert.Equal("RTX 3080", server.Gpus[0].Model);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Gpus.Count == 1 &&
            s.Gpus[0].Model == "RTX 3080"
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_if_index_out_of_range()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Gpus = new List<Gpu>
            {
                new() { Model = "RTX 4090", Vram = 24 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new RemoveGpuUseCase(repo);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await sut.ExecuteAsync("node01", 1)
        );

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }

    [Fact]
    public async Task ExecuteAsync_Does_nothing_when_server_does_not_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();

        repo.GetByNameAsync("node01")
            .Returns((Hardware?)null);

        var sut = new RemoveGpuUseCase(repo);

        // Act
        await sut.ExecuteAsync("node01", 0);

        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }
}