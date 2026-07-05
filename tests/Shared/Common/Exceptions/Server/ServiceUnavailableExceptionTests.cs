using FluentAssertions;
using HauteCouture.Shared.Common.Exceptions.Server;

namespace HauteCouture.Shared.Tests.Common.Exceptions.Server;

/// <summary>
/// Tests for the <see cref="InternalServerErrorException"/> class.
/// </summary>
public sealed class ServiceUnavailableExceptionTests
{
    [Fact]
    public void ServiceUnavailableException_Constructor_WithMessage_SetsMessageCorrectly()
    {
        // Arrange
        const string expectedMessage = "This message doesn't make any sense";

        // Act
        var exception = new ServiceUnavailableException(expectedMessage);

        // Assert
        exception.Message.Should().Be(expectedMessage);
    }

    [Fact]
    public void ServiceUnavailableException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new ServiceUnavailableException("This message doesn't make any sense");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}