using NSubstitute;
using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Servers.Drives;

namespace Tests.HardwareResources;

public class AddDrivesUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Adds_drive_when_server_exists()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Drives = new List<Drive>()
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new AddDrivesUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            "NVMe",
            2000
        );

        // Assert
        Assert.Single(server.Drives);
        Assert.Equal("NVMe", server.Drives[0].Type);
        Assert.Equal(2000, server.Drives[0].Size);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Drives.Count == 1 &&
            s.Drives[0].Type == "NVMe" &&
            s.Drives[0].Size == 2000
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Initializes_drive_list_when_null()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Drives = null
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new AddDrivesUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            "SATA",
            500
        );

        // Assert
        Assert.NotNull(server.Drives);
        Assert.Single(server.Drives);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Drives != null &&
            s.Drives.Count == 1 &&
            s.Drives[0].Type == "SATA"
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Does_nothing_when_server_does_not_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("node01").Returns((Hardware?)null);

        var sut = new AddDrivesUseCase(repo);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await sut.ExecuteAsync(
                "node01",
                "NVMe",
                2000
            )
        );

        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }
}