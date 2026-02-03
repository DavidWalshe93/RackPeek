using NSubstitute;
using RackPeek.Domain.Helpers;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.Servers.Drives;

namespace Tests.HardwareResources;

public class UpdateDriveUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Updates_drive_when_index_is_valid()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        var server = new Server
        {
            Name = "node01",
            Drives = new List<Drive>
            {
                new() { Type = "NVMe", Size = 2000 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new UpdateDriveUseCase(repo);

        // Act
        await sut.ExecuteAsync(
            "node01",
            0,
            "SATA",
            500
        );

        // Assert
        Assert.Equal("SATA", server.Drives[0].Type);
        Assert.Equal(500, server.Drives[0].Size);

        await repo.Received(1).UpdateAsync(Arg.Is<Server>(s =>
            s.Name == "node01" &&
            s.Drives.Count == 1 &&
            s.Drives[0].Type == "SATA" &&
            s.Drives[0].Size == 500
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
            Drives = new List<Drive>
            {
                new() { Type = "NVMe", Size = 2000 }
            }
        };

        repo.GetByNameAsync("node01").Returns(server);

        var sut = new UpdateDriveUseCase(repo);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await sut.ExecuteAsync(
                "node01",
                1,
                "SATA",
                500
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
            .Returns((Hardware?)null);

        var sut = new UpdateDriveUseCase(repo);

        // Act
        var ex = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await sut.ExecuteAsync(
                "node01",
                0,
                "SATA",
                500
            )
        );
        
        // Assert
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Server>());
    }
}