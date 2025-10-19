using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdatePostBrutalDTOValidator : AbstractValidator<UpdatePostBrutalDTO>
{
    public UpdatePostBrutalDTOValidator()
    {
        RuleFor(upb => upb.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(upb => upb.File)
            .NotEmpty()
            .WithMessage("File cannot be empty.")
            .Length(4, 1024)
            .WithMessage("File must be between 4-1024 characters.");
    }
}