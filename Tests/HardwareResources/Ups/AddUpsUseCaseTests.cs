using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Models;
using RackPeek.Domain.Resources.Hardware.UpsUnits;

namespace Tests.HardwareResources.Ups;

public class AddUpsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_Adds_new_ups_when_not_exists()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ups01").Returns((Hardware?)null);

        var sut = new AddUpsUseCase(repo);

        await sut.ExecuteAsync("ups01");

        await repo.Received(1).AddAsync(Arg.Is<RackPeek.Domain.Resources.Hardware.Models.Ups>(u =>
            u.Name == "ups01"
        ));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_if_ups_already_exists()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("ups01").Returns(new RackPeek.Domain.Resources.Hardware.Models.Ups { Name = "ups01" });

        var sut = new AddUpsUseCase(repo);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await sut.ExecuteAsync("ups01")
        );

        Assert.Equal("UPS 'ups01' already exists.", ex.Message);
        await repo.DidNotReceive().AddAsync(Arg.Any<RackPeek.Domain.Resources.Hardware.Models.Ups>());
    }
}