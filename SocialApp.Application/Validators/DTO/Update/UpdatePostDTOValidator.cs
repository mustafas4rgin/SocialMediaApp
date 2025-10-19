using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdatePostDTOValidator : AbstractValidator<UpdatePostDTO>
{
    public UpdatePostDTOValidator()
    {
        RuleFor(up => up.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.")
            .Length(4, 128)
            .WithMessage("Body must be between 4-128 characters.");

        RuleFor(up => up.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
    }
}