using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class PostBrutalValidator : AbstractValidator<PostBrutal>
{
    public PostBrutalValidator()
    {
        RuleFor(pb => pb.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(pb => pb.File)
            .NotEmpty()
            .WithMessage("File cannot be empty.")
            .Length(4, 1024)
            .WithMessage("File must be between 4-1024 characters.");
    }
}