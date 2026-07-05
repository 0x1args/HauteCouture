using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Client;

/// <summary>
/// Tests for the <see cref="UnauthorizedException"/> class.
/// </summary>
public sealed class UnauthorizedExceptionTests
{
    [Fact]
    public void UnauthorizedException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new UnauthorizedException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void UnauthorizedException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new UnauthorizedException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}