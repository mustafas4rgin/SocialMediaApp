using FluentValidation;
using SocialApp.Domain.DTOs.Create;

namespace SocialApp.Application.Validators.DTO.Create;

public class CreatePostImageDTOValidator : AbstractValidator<CreatePostImageDTO>
{
    public CreatePostImageDTOValidator()
    {
        RuleFor(cpi => cpi.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(cpi => cpi.File)
            .NotEmpty()
            .WithMessage("File cannot be empty.")
            .Length(4, 1024)
            .WithMessage("File must be between 4-1024 characters.");
    }
}