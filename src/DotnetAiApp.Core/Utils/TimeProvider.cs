namespace DotnetAiApp.Core.Utils;

public interface ITimeProvider
{
    DateTime CurrentTime { get; }
    DateTime CurrentDate { get; }
}

public class TimeProvider : ITimeProvider
{
    public DateTime CurrentTime => DateTime.UtcNow;
    public DateTime CurrentDate => CurrentTime.Date;
}
