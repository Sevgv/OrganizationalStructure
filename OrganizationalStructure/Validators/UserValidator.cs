using FluentValidation;
using OrganizationalStructure.Domain;

namespace OrganizationalStructure.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty()
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Please specify a first name");
        RuleFor(x => x.SecondName).NotEmpty()
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Please specify a second name");
        RuleFor(x => x.MiddleName).NotEmpty()
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Please specify a middle name");
    }
}
