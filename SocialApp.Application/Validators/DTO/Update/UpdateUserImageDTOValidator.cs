using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdateUserImageDTOValidator : AbstractValidator<UpdateUserImageDTO>
{
    public UpdateUserImageDTOValidator()
    {
        RuleFor(uui => uui.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
        
        RuleFor(uui => uui.File)
            .NotEmpty()
            .WithMessage("File cannot be empty.")
            .Length(4, 1024)
            .WithMessage("File must be between 4-1024 characters.");
    }
}