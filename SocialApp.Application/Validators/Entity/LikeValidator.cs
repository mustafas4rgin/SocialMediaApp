using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class LikeValidator : AbstractValidator<Like>
{
    public LikeValidator()
    {
        RuleFor(l => l.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(l => l.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
    }
}