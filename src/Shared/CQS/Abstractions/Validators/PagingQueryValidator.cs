using FluentValidation;
using HauteCouture.Shared.CQS.Primitives.Queries;

namespace HauteCouture.Shared.CQS.Abstractions.Validators;

public abstract class PagingQueryValidator : AbstractValidator<IPagedQuery>
{
    protected PagingQueryValidator()
    {
    }
}