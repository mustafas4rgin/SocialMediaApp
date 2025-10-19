using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators.DTO.Create;

public class CreateUserImageDTOValidator : AbstractValidator<CreateUserImageDTO>
{
    public CreateUserImageDTOValidator()
    {
        RuleFor(cui => cui.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
        
        RuleFor(cui => cui.File)
            .NotEmpty()
            .WithMessage("File cannot be empty.")
            .Length(4, 1024)
            .WithMessage("File must be between 4-1024 characters.");
    }
}