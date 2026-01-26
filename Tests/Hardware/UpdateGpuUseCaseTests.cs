using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Server.Gpu;

namespace Tests.Hardware;

public class UpdateGpuUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Updates_gpu_when_index_is_valid()
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

        var sut = new UpdateGpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            0,
            "RTX 3080",
            10
        );

        // Assert
        Assert.Equal("RTX 3080", server.Gpus[0].Model);
        Assert.Equal(10, server.Gpus[0].Vram);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Gpus.Count == 1 &&
            s.Gpus[0].Model == "RTX 3080" &&
            s.Gpus[0].Vram == 10
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

        var sut = new UpdateGpuUseCase(repo);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await sut.ExecuteAsync(
                "node01",
                1,
                "RTX 3080",
                10
            )
        );

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }

    [Fact]
    public async Task ExecuteAsync_Does_nothing_when_server_does_not_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();

        repo.GetByNameAsync("node01")
            .Returns((RackPeek.Domain.Resources.Hardware.Models.Hardware?)null);

        var sut = new UpdateGpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            0,
            "RTX 3080",
            10
        );

        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }
}
