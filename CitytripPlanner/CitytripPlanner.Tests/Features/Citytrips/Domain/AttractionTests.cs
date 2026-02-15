namespace CitytripPlanner.Tests.Features.Citytrips.Domain;

using CitytripPlanner.Features.Citytrips.Domain;
using FluentAssertions;

public class AttractionTests
{
    [Fact]
    public void Constructor_ValidInputs_CreatesAttraction()
    {
        // Arrange & Act
        var attraction = new Attraction(
            "Eiffel Tower",
            "Iconic iron lattice tower",
            "https://www.toureiffel.paris/en",
            new List<string> { "Metro Line 6 to Bir-Hakeim" }
        );

        // Assert
        attraction.Name.Should().Be("Eiffel Tower");
        attraction.Description.Should().Be("Iconic iron lattice tower");
        attraction.WebsiteUrl.Should().Be("https://www.toureiffel.paris/en");
        attraction.TransportationOptions.Should().ContainSingle();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Act & Assert
        var act = () => new Attraction(invalidName!, "Description", null, new List<string>());
        act.Should().Throw<ArgumentException>().WithMessage("*Name*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidDescription_ThrowsArgumentException(string? invalidDescription)
    {
        // Act & Assert
        var act = () => new Attraction("Name", invalidDescription!, null, new List<string>());
        act.Should().Throw<ArgumentException>().WithMessage("*Description*");
    }

    [Fact]
    public void Constructor_InvalidWebsiteUrl_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new Attraction("Name", "Description", "not-a-valid-url", new List<string>());
        act.Should().Throw<ArgumentException>().WithMessage("*WebsiteUrl*");
    }

    [Fact]
    public void Constructor_NullWebsiteUrl_DoesNotThrow()
    {
        // Act
        var act = () => new Attraction("Name", "Description", null, new List<string>());

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_NullTransportationOptions_ThrowsArgumentNullException()
    {
        // Act & Assert
        var act = () => new Attraction("Name", "Description", null, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("transportationOptions");
    }

    [Fact]
    public void Constructor_EmptyTransportationOptions_DoesNotThrow()
    {
        // Act
        var act = () => new Attraction("Name", "Description", null, new List<string>());

        // Assert
        act.Should().NotThrow();
    }
}
