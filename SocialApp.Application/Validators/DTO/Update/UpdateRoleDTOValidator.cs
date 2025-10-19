using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdateRoleDTOValidator : AbstractValidator<UpdateRoleDTO>
{
    public UpdateRoleDTOValidator()
    {
        RuleFor(ur => ur.Name)
            .NotEmpty()
            .WithMessage("Name value cannot be empty.")
            .Length(4, 128)
            .WithMessage("Name value must be between 4-128 characters.");
    }
}