using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Client;

/// <summary>
/// Tests for the <see cref="ConflictException"/> class.
/// </summary>
public sealed class ConflictExceptionTests
{
    [Fact]
    public void ConflictException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new ConflictException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void ConflictException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new ConflictException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}