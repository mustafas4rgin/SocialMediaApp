using FluentValidation;
using SocialApp.Domain.DTOs.Auth;

namespace SocialApp.Application.Validators.DTO.Auth;

public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    public RegisterDTOValidator()
    {
        RuleFor(rdto => rdto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters.")
            .Must(x => x.Trim().Length == x.Length)
                .WithMessage("Email cannot start or end with spaces.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_.-]*$")
                .WithMessage("Username can contain letters, numbers, _, ., - characters only.")
            .Must(x => x.Trim().Length == x.Length)
                .WithMessage("Username cannot start or end with spaces.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.")
            .Must(x => x.Trim().Length == x.Length)
                .WithMessage("First name cannot have leading or trailing spaces.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
            .Must(x => x.Trim().Length == x.Length)
                .WithMessage("Last name cannot have leading or trailing spaces.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is necessary.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.PasswordMatch)
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}
