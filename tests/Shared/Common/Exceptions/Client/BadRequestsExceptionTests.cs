using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Client;

/// <summary>
/// Tests for the <see cref="BadRequestException"/> class.
/// </summary>
public sealed class BadRequestExceptionTests
{
    [Fact]
    public void BadRequestException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new BadRequestException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void BadRequestException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new BadRequestException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}