using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class UserImageValidator : AbstractValidator<UserImage>
{
    public UserImageValidator()
    {
        RuleFor(ui => ui.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
        
        RuleFor(ui => ui.File)
            .NotEmpty()
            .WithMessage("File cannot be empty.")
            .Length(4, 1024)
            .WithMessage("File must be between 4-1024 characters.");
    }
}