using NSubstitute;
using RackPeek.Domain.Resources.Hardware;
using RackPeek.Domain.Resources.Hardware.Desktops;
using RackPeek.Domain.Resources.Hardware.Models;

namespace Tests.HardwareResources.Desktops;

public class GetDesktopsUseCaseTests
{
    [Fact]
    public async Task Returns_All_Desktops()
    {
        var repo = Substitute.For<IHardwareRepository>();
        repo.GetAllAsync().Returns(new Hardware[]
        {
            new Desktop { Name = "desk1" },
            new Desktop { Name = "desk2" }
        });

        var useCase = new GetDesktopsUseCase(repo);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Count);
    }
}