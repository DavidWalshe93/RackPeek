using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;

public class DeleteDesktopUseCaseTests
{
    [Fact]
    public async Task Deletes_Desktop()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("desk1").Returns(new Desktop { Name = "desk1" });

        var useCase = new DeleteDesktopUseCase(repo);

        await useCase.ExecuteAsync("desk1");

        await repo.Received().DeleteAsync("desk1");
    }

    [Fact]
    public async Task Throws_If_Not_Found()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("desk1").Returns((Hardware?)null);

        var useCase = new DeleteDesktopUseCase(repo);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync("desk1"));
    }
}