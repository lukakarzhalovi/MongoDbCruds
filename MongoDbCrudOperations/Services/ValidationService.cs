using System.ComponentModel.DataAnnotations;

namespace MongoDbCrudOperations.Services;

public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T model) where T : class;
    bool TryParseInt(string input, out int result);
    bool TryParseDateTime(string input, out DateTime result);
}

public class ValidationService : IValidationService
{
    public Task<ValidationResult> ValidateAsync<T>(T model) where T : class
    {
        var context = new ValidationContext(model);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = Validator.TryValidateObject(model, context, results, true);

        if (!isValid)
        {
            var errorMessages = results.Select(r => r.ErrorMessage).Where(m => !string.IsNullOrEmpty(m));
            return Task.FromResult(new ValidationResult(false, string.Join("; ", errorMessages)));
        }

        return Task.FromResult(ValidationResult.Success!);
    }

    public bool TryParseInt(string input, out int result)
    {
        return int.TryParse(input, out result);
    }

    public bool TryParseDateTime(string input, out DateTime result)
    {
        return DateTime.TryParse(input, out result);
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    public ValidationResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success => new(true);
    public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
}