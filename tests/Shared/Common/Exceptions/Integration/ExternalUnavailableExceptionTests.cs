using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Integration;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Integration;

/// <summary>
/// Tests for the <see cref="ExternalUnavailableException"/> class.
/// </summary>
public sealed class ExternalUnavailableExceptionTests
{
    [Fact]
    public void ExternalUnavailableException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new ExternalUnavailableException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void ExternalUnavailableException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new ExternalUnavailableException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}