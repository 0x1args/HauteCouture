using FluentValidation;
using HauteCouture.Example.Domain.ValueObjects;

namespace HauteCouture.Example.Applications.Handlers.Commands.CreateSomething;

/// <summary>
///     Validates <see cref="CreateSomethingCommand"/>.
/// </summary>
public sealed class CreateSomethingCommandValidator 
    : AbstractValidator<CreateSomethingCommand>
{
    public CreateSomethingCommandValidator()
    {
        RuleFor(c => c.Request.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(SomethingName.MaxLength).WithMessage("Name must not exceed {MaxLength} characters.");
        RuleFor(c => c.Request.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(SomethingDescription.MaxLength).WithMessage("Description must not exceed {MaxLength} characters.");
        RuleFor(c => c.Request.Price)
            .GreaterThan(0).WithMessage("Price must be a positive value.");
        RuleFor(c => c.UserId)
            .NotEqual(Guid.Empty).WithMessage("User ID is required.");
    }
}