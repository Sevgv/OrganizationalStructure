using FluentValidation;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Validators;

public class OrgStructureValidator : AbstractValidator<OrgStructure>
{
    public OrgStructureValidator(
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        IUserRepository userRepository)
    {

        RuleFor(x => x.DepartmentId)
            .Must(departmentId => departmentRepository.IsExists(d => d.Id == departmentId).Result)
            .WithMessage("Department must be exists");

        RuleFor(x => x.PositionId)
            .Must(positionId => positionRepository.IsExists(d => d.Id == positionId).Result)
            .WithMessage("Parent department must be exists");

        RuleFor(x => x.UserId)
            .Must(userId => userRepository.IsExists(d => d.Id == userId).Result)
            .WithMessage("Parent department must be exists");
    }
}
