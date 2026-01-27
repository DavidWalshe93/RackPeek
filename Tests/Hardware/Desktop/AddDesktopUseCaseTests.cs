using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Desktop;
using RackPeek.Domain.Resources.Hardware.Models;

public class AddDesktopUseCaseTests
{
    [Fact]
    public async Task Adds_New_Desktop()
    {
        var repo = Substitute.For<IHardwareR epository>();
        repo.GetByNameAsync("desk1").Returns((Hardware?)null);

        var useCase = new AddDesktopUseCase(repo);

        await useCase.ExecuteAsync("desk1");

        await repo.Received().AddAsync(Arg.Is<Desktop>(d => d.Name == "desk1"));
    }

    [Fact]
    public async Task Throws_If_Desktop_Exists()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetByNameAsync("desk1").Returns(new Desktop { Name = "desk1" });

        var useCase = new AddDesktopUseCase(repo);

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync("desk1"));
    }
}