namespace DotnetAiApp.Core.Utils;

public interface ITimeProvider
{
    DateTime CurrentTime { get; }
    DateOnly CurrentDate { get; }
}

public class TimeProvider : ITimeProvider
{
    public DateTime CurrentTime => DateTime.UtcNow;
    public DateOnly CurrentDate => DateOnly.FromDateTime(CurrentTime);
}
