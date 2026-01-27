using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.UpsUnits;

namespace Tests.HardwareResources.Ups;

public class UpdateUpsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Throws_when_ups_not_found()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ups01").Returns((Hardware?)null);

        var sut = new UpdateUpsUseCase(repo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sut.ExecuteAsync("ups01")
        );

        Assert.Equal("UPS 'ups01' not found.", ex.Message);
        await repo.DidNotReceive().UpdateAsync(Arg.Any<RackPeek.Domain.Resources.Hardware.Models.Ups>());
    }

    [Fact]
    public async Task ExecuteAsync_Updates_only_provided_fields()
    {
        var existing = new RackPeek.Domain.Resources.Hardware.Models.Ups
        {
            Name = "ups01",
            Model = "OldModel",
            Va = 1000
        };

        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ups01").Returns(existing);

        var sut = new UpdateUpsUseCase(repo);

        await sut.ExecuteAsync(
            "ups01",
            "NewModel",
            1500
        );

        await repo.Received(1).UpdateAsync(Arg.Is<RackPeek.Domain.Resources.Hardware.Models.Ups>(u =>
            u.Name == "ups01" &&
            u.Model == "NewModel" &&
            u.Va == 1500
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Does_not_update_model_when_empty_or_whitespace()
    {
        var existing = new RackPeek.Domain.Resources.Hardware.Models.Ups
        {
            Name = "ups01",
            Model = "KeepMe",
            Va = 1000
        };

        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ups01").Returns(existing);

        var sut = new UpdateUpsUseCase(repo);

        await sut.ExecuteAsync(
            "ups01",
            "   "
        );

        await repo.Received(1).UpdateAsync(Arg.Is<RackPeek.Domain.Resources.Hardware.Models.Ups>(u =>
            u.Model == "KeepMe"
        ));
    }
}