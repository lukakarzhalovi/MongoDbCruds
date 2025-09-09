using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace MongoDbCrudOperations.Services;

public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T model) where T : class;
    bool TryParseInt(string input, out int result);
    bool TryParseDateTime(string input, out DateTime result);
}

public class ValidationService(IServiceProvider serviceProvider) : IValidationService
{
    public async Task<ValidationResult> ValidateAsync<T>(T model) where T : class
    {
        var validator = serviceProvider.GetService(typeof(IValidator<T>)) as IValidator<T>;
        if (validator != null)
        {
            var fluentResult = await validator.ValidateAsync(model, CancellationToken.None);
            if (!fluentResult.IsValid)
            {
                var errors = fluentResult.Errors.Select(e => e.ErrorMessage);
                return new ValidationResult(false, string.Join("; ", errors));
            }
        }

        var context = new ValidationContext(model);
        var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = Validator.TryValidateObject(model, context, results, true);

        if (!isValid)
        {
            var errorMessages = results.Select(r => r.ErrorMessage).Where(m => !string.IsNullOrEmpty(m));
            return new ValidationResult(false, string.Join("; ", errorMessages));
        }

        return ValidationResult.Success!;
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
