using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Client;

/// <summary>
/// Tests for the <see cref="TooManyRequestsException"/> class.
/// </summary>
public sealed class RateLimitExceptionTests
{
    [Fact]
    public void RateLimitException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new TooManyRequestsException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void RateLimitException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new TooManyRequestsException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}