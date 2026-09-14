using System.ComponentModel.DataAnnotations;

namespace EventManager.UnitTests.Validation;

public abstract class ValidationTestBase
{
    protected static IReadOnlyList<ValidationResult> Validate<TModel>(TModel dto) where TModel : class
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results;
    }

    protected static void AssertErrors(string[] expectedErrors, IReadOnlyList<ValidationResult> validationResults)
    {
        Assert.Equal(expectedErrors.Order(), validationResults.Select(r => r.ErrorMessage).Order());
    }
}
