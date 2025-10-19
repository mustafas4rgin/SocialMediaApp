using FluentValidation;
using SocialApp.Domain.DTOs.Create;

namespace SocialApp.Application.Validators.DTO.Create;

public class CreatePostDTOValidator : AbstractValidator<CreatePostDTO>
{
    public CreatePostDTOValidator()
    {
        RuleFor(cp => cp.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.")
            .Length(4, 128)
            .WithMessage("Body must be between 4-128 characters.");

        RuleFor(cp => cp.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
    }
}