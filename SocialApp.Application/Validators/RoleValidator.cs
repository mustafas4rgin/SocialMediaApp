using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class RoleValidator : AbstractValidator<Role>
{
    public RoleValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .WithMessage("Name value cannot be empty.")
            .Length(4, 128)
            .WithMessage("Name value must be between 4-128 characters.");
    }
}