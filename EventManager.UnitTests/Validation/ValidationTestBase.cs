using System.ComponentModel.DataAnnotations;

namespace EventManager.UnitTests.Validation;

public abstract class ValidationTestBase
{
    protected static IReadOnlyList<ValidationResult> Validate<TModel>(TModel model) where TModel : class
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);

        return results;
    }

    protected static void AssertValidationResults(string[] expectedErrors, IReadOnlyList<ValidationResult> validationResults)
    {
        Assert.Equal(expectedErrors.Order(), validationResults.Select(r => r.ErrorMessage).Order());
    }
}
