using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Client;

/// <summary>
/// Tests for the <see cref="NotFoundException"/> class.
/// </summary>
public sealed class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new NotFoundException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new NotFoundException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}