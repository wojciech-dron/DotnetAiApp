using DotnetAiApp.Core.Utils;

namespace DotnetAiApp.Tests.Mocks;

public class TimeProviderMock : ITimeProvider
{
    public DateTime CurrentTime { get; set; } = new(2025, 1, 1);
    public DateTime CurrentDate { get; set; } = new(2025, 1, 1);
}