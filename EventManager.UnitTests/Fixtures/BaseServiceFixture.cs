using EventManager.Application.Services.Interfaces;
using Moq;

namespace EventManager.UnitTests.Fixtures;

public abstract class BaseServiceFixture
{
    public Mock<IDateTimeProvider> DateTimeProviderMock { get; } = new();
    protected IDateTimeProvider DateTimeProvider => DateTimeProviderMock.Object;

    public BaseServiceFixture()
    {
        DateTimeProviderMock.Setup(p => p.Now).Returns(DateTime.UtcNow);
        DateTimeProviderMock.Setup(p => p.Today).Returns(DateTime.UtcNow.Date);
    }
}
