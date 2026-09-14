using EventManager.Application.Models.DTO.Events;

namespace EventManager.UnitTests.Validation;

[Collection("EventDtoValidation")]
public class UpdateEventDtoValidationTests : ValidationTestBase
{
    public static IEnumerable<object?[]> Validate_Title_ReturnsExpected_TestData() =>
    [
        [null, new string[] { "The Title field is required." }],
        ["", new string[] { "The Title field is required." }],
        [new string('A', 101), new string[] { "The field Title must be a string with a minimum length of 1 and a maximum length of 100." }],
        [new string('A', 100), Array.Empty<string>()],
        ["A", Array.Empty<string>()]
    ];

    [Theory]
    [MemberData(nameof(Validate_Title_ReturnsExpected_TestData))]
    public void Validate_Title_ReturnsExpected(string? title, string[] expectedErrors)
    {
        // Arrange
        var eventDto = new UpdateEventDto
        {
            Id = Guid.NewGuid(),
            Title = title!,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var validationResults = Validate(eventDto);

        // Assert
        AssertErrors(expectedErrors, validationResults);
    }

    public static IEnumerable<object?[]> Validate_Description_ReturnsExpected_TestData() =>
    [
        [null, Array.Empty<string>()],
        ["", Array.Empty<string>()],
        [new string('A', 301), new string[] { "The field Description must be a string with a maximum length of 300." }]
    ];

    [Theory]
    [MemberData(nameof(Validate_Description_ReturnsExpected_TestData))]
    public void Validate_Description_ReturnsExpected(string? description, string[] expectedErrors)
    {
        // Arrange
        var eventDto = new UpdateEventDto
        {
            Id = Guid.NewGuid(),
            Title = "Event",
            Description = description,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var validationResults = Validate(eventDto);

        // Assert
        AssertErrors(expectedErrors, validationResults);
    }

    public static IEnumerable<object?[]> Validate_Dates_ReturnsExpected_TestData() =>
    [
        [null, null, new string[] { "The StartAt field is required.", "The EndAt field is required." }],
        [DateTime.Today, DateTime.Today, new string[] { "EndAt must be greater than StartAt" }],
        [DateTime.Today.AddMinutes(1), DateTime.Today, new string[] { "EndAt must be greater than StartAt" }],
        [DateTime.MinValue, DateTime.MaxValue, Array.Empty<string>()]
    ];

    [Theory]
    [MemberData(nameof(Validate_Dates_ReturnsExpected_TestData))]
    public void Validate_Dates_ReturnsExpected(DateTime? startAt, DateTime? endAt, string[] expectedErrors)
    {
        // Arrange
        var eventDto = new UpdateEventDto
        {
            Id = Guid.NewGuid(),
            Title = "Event",
            StartAt = startAt,
            EndAt = endAt
        };

        // Act
        var validationResults = Validate(eventDto);

        // Assert
        AssertErrors(expectedErrors, validationResults);
    }
}
