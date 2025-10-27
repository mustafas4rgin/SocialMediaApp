using FluentValidation;
using SocialApp.Domain.DTOs.Auth;

namespace SocialApp.Application.Validators.DTO.Auth;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Must be a valid email address.")
            .Length(6, 50)
            .WithMessage("Email must be between 6-50 characters.");

        RuleFor(l => l.Password)
            .NotEmpty()
            .WithMessage("Password cannot be empty.")
            .MinimumLength(8)
            .WithMessage("Password must be atlest 8 characters.");
    }
}