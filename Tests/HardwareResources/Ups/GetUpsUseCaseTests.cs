using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.UpsUnits;

namespace Tests.HardwareResources.Ups;

public class GetUpsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Returns_only_ups_units()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetAllAsync().Returns([
            new RackPeek.Domain.Resources.Hardware.Models.Ups { Name = "ups01" },
            new Server { Name = "node01" },
            new RackPeek.Domain.Resources.Hardware.Models.Ups { Name = "ups02" }
        ]);

        var sut = new GetUpsUseCase(repo);

        var result = await sut.ExecuteAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, u => Assert.IsType<RackPeek.Domain.Resources.Hardware.Models.Ups>(u));
        Assert.Contains(result, u => u.Name == "ups01");
        Assert.Contains(result, u => u.Name == "ups02");
    }

    [Fact]
    public async Task ExecuteAsync_Returns_empty_list_when_no_ups_exist()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetAllAsync().Returns([
            new Server { Name = "node01" }
        ]);

        var sut = new GetUpsUseCase(repo);

        var result = await sut.ExecuteAsync();

        Assert.Empty(result);
    }
}