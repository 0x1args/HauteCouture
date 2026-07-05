using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Client;

/// <summary>
/// Tests for the <see cref="GoneException"/> class.
/// </summary>
public sealed class GoneExceptionTests
{
    [Fact]
    public void GoneException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new GoneException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void GoneException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new GoneException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}