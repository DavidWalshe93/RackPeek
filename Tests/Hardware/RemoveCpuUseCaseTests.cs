using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Server.Cpu;
using Xunit;

namespace Tests.Hardware;

public class RemoveCpuUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Removes_cpu_when_index_is_valid()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Cpus = new List<Cpu>
            {
                new Cpu { Model = "7950x", Cores = 8, Threads = 16 },
                new Cpu { Model = "7900x", Cores = 12, Threads = 24 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new RemoveCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync("node01", index: 0);

        // Assert
        Assert.Single(server.Cpus);
        Assert.Equal("7900x", server.Cpus[0].Model);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Cpus.Count == 1 &&
            s.Cpus[0].Model == "7900x"
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

        var sut = new RemoveCpuUseCase(repo);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await sut.ExecuteAsync("node01", index: 1)
        );

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }

    [Fact]
    public async Task ExecuteAsync_Does_nothing_when_server_does_not_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("node01").Returns((RackPeek.Domain.Resources.Hardware.Models.Hardware?)null);

        var sut = new RemoveCpuUseCase(repo);

        // Act
        await sut.ExecuteAsync("node01", index: 0);

        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<RackPeek.Domain.Resources.Hardware.Models.Server>());
    }
}
