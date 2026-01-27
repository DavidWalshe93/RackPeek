using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.AccessPoints;
using RackPeek.Domain.Resources.Hardware.Models;

namespace Tests.HardwareResources.AccessPoints;

public class GetAccessPointsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_only_access_points()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetAllAsync().Returns([
            new AccessPoint { Name = "ap01" },
            new Server { Name = "node01" },
            new AccessPoint { Name = "ap02" }
        ]);

        var sut = new GetAccessPointsUseCase(repo);

        // Act
        var result = await sut.ExecuteAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, ap => Assert.IsType<AccessPoint>(ap));
        Assert.Contains(result, ap => ap.Name == "ap01");
        Assert.Contains(result, ap => ap.Name == "ap02");
    }

    [Fact]
    public async Task ExecuteAsync_Returns_empty_list_when_no_access_points_exist()
    {
        // Arrange
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetAllAsync().Returns([
            new Server { Name = "node01" }
        ]);

        var sut = new GetAccessPointsUseCase(repo);

        // Act
        var result = await sut.ExecuteAsync();

        // Assert
        Assert.Empty(result);
    }
}