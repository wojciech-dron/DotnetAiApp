namespace NbpApp.Utils.Utils;

public class AppResult
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
    public string? ErrorMessage { get; init; }
    public IEnumerable<ValidationError>? ValidationErrors { get; set; }
}

public record ValidationError(string PropertyName, string ErrorMessage);