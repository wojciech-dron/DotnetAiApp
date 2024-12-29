namespace NbpApp.Web.Common;

public class AppResult
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public string? ErrorMessage { get; init; }
}