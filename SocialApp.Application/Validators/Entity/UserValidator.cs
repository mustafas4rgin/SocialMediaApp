using FluentValidation;
using SocialApp.Domain.Entities;

public sealed class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.PasswordHash)
            .NotNull().WithMessage("Password hash cannot be null.")
            .Must(h => h.Length > 0).WithMessage("Password hash cannot be empty.");

        RuleFor(x => x.PasswordSalt)
            .NotNull().WithMessage("Password salt cannot be null.")
            .Must(s => s.Length > 0).WithMessage("Password salt cannot be empty.");
    }
}
