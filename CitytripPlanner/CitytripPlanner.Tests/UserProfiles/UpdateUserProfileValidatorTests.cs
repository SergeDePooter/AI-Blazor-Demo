using CitytripPlanner.Features.UserProfiles.Domain;
using CitytripPlanner.Features.UserProfiles.UpdateUserProfile;

namespace CitytripPlanner.Tests.UserProfiles;

public class UpdateUserProfileValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReturnsError()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("", "John", null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_EmptyFirstname_ReturnsError()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "", null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Firstname");
    }

    [Fact]
    public void Validate_NameTooLong_ReturnsError()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var longName = new string('a', 101); // 101 characters
        var command = new UpdateUserProfileCommand(longName, "John", null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_FirstnameTooLong_ReturnsError()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var longFirstname = new string('b', 101); // 101 characters
        var command = new UpdateUserProfileCommand("Doe", longFirstname, null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Firstname" && e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_ValidNameAndFirstname_Passes()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "John", null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_Exactly100Characters_Passes()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var name100 = new string('a', 100);
        var firstname100 = new string('b', 100);
        var command = new UpdateUserProfileCommand(name100, firstname100, null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ValidGenderOption_Passes()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "John", GenderOptions.Male, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_InvalidGender_ReturnsError()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "John", "InvalidGender", null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Gender");
    }

    [Fact]
    public void Validate_ValidCountry_Passes()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "John", null, "United States");

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_InvalidCountry_ReturnsError()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "John", null, "InvalidCountry");

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Country");
    }

    [Fact]
    public void Validate_NullGenderAndCountry_Passes()
    {
        // Arrange
        var validator = new UpdateUserProfileValidator();
        var command = new UpdateUserProfileCommand("Doe", "John", null, null);

        // Act
        var result = validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
