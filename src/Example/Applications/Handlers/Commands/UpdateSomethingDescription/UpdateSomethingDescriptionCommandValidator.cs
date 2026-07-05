using FluentValidation;
using HauteCouture.Example.Domain.ValueObjects;

namespace HauteCouture.Example.Applications.Handlers.Commands.UpdateSomethingDescription;

/// <summary>
///     Validates <see cref="UpdateSomethingDescriptionCommand"/>.
/// </summary>
public sealed class UpdateSomethingDescriptionCommandValidator
    : AbstractValidator<UpdateSomethingDescriptionCommand>
{
    public UpdateSomethingDescriptionCommandValidator()
    {
        RuleFor(c => c.SomethingId)
            .NotEmpty().WithMessage("Something ID must not be empty.");
        RuleFor(c => c.Request.NewDescription)
            .NotEmpty().WithMessage("New description must not be empty.")
            .MaximumLength(SomethingDescription.MaxLength).WithMessage("New description must not exceed {MaxLength} characters.");
    }
}