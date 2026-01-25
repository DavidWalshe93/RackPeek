using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Server.Cpu;
using Xunit;

namespace Tests.Hardware;

public class UpdateCpuUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Updates_cpu_when_index_is_valid()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Cpus = new List<Cpu>
            {
                new Cpu { Model = "7950x", Cores = 8, Threads = 16 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new UpdateCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            serverName: "node01",
            index: 0,
            model: "7900x",
            cores: 12,
            threads: 24
        );

        // Assert
        Assert.Equal("7900x", server.Cpus[0].Model);
        Assert.Equal(12, server.Cpus[0].Cores);
        Assert.Equal(24, server.Cpus[0].Threads);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Cpus.Count == 1 &&
            s.Cpus[0].Model == "7900x" &&
            s.Cpus[0].Cores == 12 &&
            s.Cpus[0].Threads == 24
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
            Cpus = new List<Cpu>
            {
                new Cpu { Model = "7950x", Cores = 8, Threads = 16 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new UpdateCpuUseCase(repo);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await sut.ExecuteAsync(
                serverName: "node01",
                index: 1,
                model: "7900x",
                cores: 12,
                threads: 24
            )
        );

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }

    [Fact]
    public async Task ExecuteAsync_Does_nothing_when_server_does_not_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("node01").Returns((RackPeek.Domain.Resources.Hardware.Models.Hardware?)null);

        var sut = new UpdateCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            serverName: "node01",
            index: 0,
            model: "7900x",
            cores: 12,
            threads: 24
        );

        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }
}
