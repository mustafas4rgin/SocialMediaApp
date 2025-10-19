using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class FollowValidator : AbstractValidator<Follow>
{
    public FollowValidator()
    {
        RuleFor(f => f.FollowerId)
            .NotNull()
            .WithMessage("FollowerId value must be greater than zero.")
            .GreaterThan(0)
            .WithMessage("FollowrId value must be greater than zero.");

        RuleFor(f => f.FollowingId)
            .NotNull()
            .WithMessage("FollowingId cannot be null.")
            .GreaterThan(0)
            .WithMessage("FollowingId must be greater than zero.");
    }
}