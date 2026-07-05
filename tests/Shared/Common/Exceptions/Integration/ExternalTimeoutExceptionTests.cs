using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Integration;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Integration;

/// <summary>
/// Tests for the <see cref="ExternalTimeoutException"/> class.
/// </summary>
public sealed class ExternalTimeoutExceptionTests
{
    [Fact]
    public void ExternalTimeoutException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new ExternalTimeoutException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void ExternalTimeoutException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new ExternalTimeoutException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}