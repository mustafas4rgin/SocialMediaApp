using FluentValidation;
using SocialApp.Domain.DTOs.Create;

namespace SocialApp.Application.Validators.DTO;

public class CreateRoleDTOValidator : AbstractValidator<CreateRoleDTO>
{
    public CreateRoleDTOValidator()
    {
        RuleFor(cr => cr.Name)
            .NotEmpty()
            .WithMessage("Name value cannot be empty.")
            .Length(4, 128)
            .WithMessage("Name value must be between 4-128 characters.");
    }
}