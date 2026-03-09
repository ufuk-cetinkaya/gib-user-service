using FluentValidation;
using Application.Requests;
using Domain.Enums;

namespace Api.Validators;

public class GetGibUserRequestValidator : AbstractValidator<GetGibUserRequest>
{
    public GetGibUserRequestValidator()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty()
            .Matches(@"^\d{10,11}$")
            .WithMessage("TaxNumber must be 10 or 11 digits");

        RuleFor(x => x.DocumentType)
            .Must(v => Enum.TryParse<DocType>(v, true, out _))
            .WithMessage("DocumentType must be INVOICE or DESPATCHADVICE");

        RuleFor(x => x.Unit)
            .Must(v => Enum.TryParse<Unit>(v, true, out _))
            .WithMessage("Unit must be GB or PK");
    }
}