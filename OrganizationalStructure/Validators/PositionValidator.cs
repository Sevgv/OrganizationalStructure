using FluentValidation;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Validators;

public class PositionValidator : AbstractValidator<Position>
{
    public PositionValidator(IPositionRepository positionRepository)
    {
        RuleFor(x => x.Name).NotEmpty()
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Please specify a position name");
        RuleFor(x => x.Name)
            .Must(name => !positionRepository.IsExists(d => d.Name == name).Result)
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Position with this name already exists");
    }
}
