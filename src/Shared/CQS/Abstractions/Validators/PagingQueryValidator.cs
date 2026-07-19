using FluentValidation;
using HauteCouture.Shared.CQS.Primitives.Queries;

namespace HauteCouture.Shared.CQS.Abstractions.Validators;

/// <summary>
///     Base validator for queries implementing <see cref="IPagedQuery"/>.
/// </summary>
public abstract class PagingQueryValidator : AbstractValidator<IPagedQuery>
{
    /// <summary>
    ///     Maximum allowed page size, to protect against excessively large result sets.
    /// </summary>
    private const int MaxPageSize = 100;

    protected PagingQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(MaxPageSize).WithMessage("Page size must not exceed {MaxPageSize} items.");
    }
}