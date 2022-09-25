using FluentValidation;
using OrganizationalStructure.Domain;
using OrganizationalStructure.Infrastructure.Repositories.Contracts;

namespace OrganizationalStructure.Validators;

public class DepartmentValidator : AbstractValidator<Department>
{
    public DepartmentValidator(IDepartmentRepository departmentRepository)
    {
        RuleFor(x => x.Name).NotEmpty()
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Please specify a department name");
        RuleFor(x => x.Name)
            .Must(name => !departmentRepository.IsExists(d => d.Name == name).Result)
            .When(x => x.Id == Guid.Empty)
            .WithMessage("Department with this name already exists");

        RuleFor(x => x.ParentDepartmentId)
            .Must(parentId => parentId == null || departmentRepository.IsExists(d => d.Id == parentId).Result)
            .WithMessage("Parent department must be exists");
    }
}
