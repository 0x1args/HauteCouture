using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Integration;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Integration;

/// <summary>
/// Tests for the <see cref="ExternalAuthException"/> class.
/// </summary>
public sealed class ExternalAuthExceptionTests
{
    [Fact]
    public void ExternalAuthException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new ExternalAuthException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void ExternalAuthException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new ExternalAuthException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}