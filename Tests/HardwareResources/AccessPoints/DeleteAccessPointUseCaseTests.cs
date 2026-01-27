using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.AccessPoints;
using RackPeek.Domain.Resources.Hardware.Models;

namespace Tests.HardwareResources.AccessPoints;

public class DeleteAccessPointUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Deletes_ap_when_exists()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ap01").Returns(new AccessPoint { Name = "ap01" });

        var sut = new DeleteAccessPointUseCase(repo);

        // Act
        await sut.ExecuteAsync("ap01");

        // Assert
        await repo.Received(1).DeleteAsync("ap01");
    }

    [Fact]
    public async Task ExecuteAsync_Throws_if_ap_not_found()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ap01").Returns((Hardware?)null);

        var sut = new DeleteAccessPointUseCase(repo);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sut.ExecuteAsync("ap01")
        );

        // Assert
        Assert.Equal("Access point 'ap01' not found.", ex.Message);
        await repo.DidNotReceive().DeleteAsync(Arg.Any<string>());
    }
}