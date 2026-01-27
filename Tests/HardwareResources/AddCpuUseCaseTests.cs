using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Servers.Cpus;

namespace Tests.HardwareResources;

public class AddCpuUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Adds_cpu_when_server_exists()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Cpus = new List<Cpu>()
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new AddCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            "7950x",
            8,
            16
        );

        // Assert
        Assert.Single(server.Cpus);
        Assert.Equal("7950x", server.Cpus[0].Model);
        Assert.Equal(8, server.Cpus[0].Cores);
        Assert.Equal(16, server.Cpus[0].Threads);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Cpus.Count == 1 &&
            s.Cpus[0].Model == "7950x" &&
            s.Cpus[0].Cores == 8 &&
            s.Cpus[0].Threads == 16
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Initializes_cpu_list_when_null()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Cpus = null
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new AddCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            "7950x",
            8,
            16
        );

        // Assert
        Assert.NotNull(server.Cpus);
        Assert.Single(server.Cpus);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Cpus != null &&
            s.Cpus.Count == 1 &&
            s.Cpus[0].Model == "7950x"
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Does_nothing_when_server_does_not_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("node01").Returns((Hardware?)null);

        var sut = new AddCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            "7950x",
            8,
            16
        );

        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }
}