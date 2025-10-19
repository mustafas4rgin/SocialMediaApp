using FluentValidation;
using SocialApp.Domain.DTOs.Create;

namespace SocialApp.Application.Validators.DTO.Create;

public class CreateFollowDTOValidator : AbstractValidator<CreateFollowDTO>
{
    public CreateFollowDTOValidator()
    {
        RuleFor(cf => cf.FollowerId)
            .NotNull()
            .WithMessage("FollowerId value must be greater than zero.")
            .GreaterThan(0)
            .WithMessage("FollowrId value must be greater than zero.");

        RuleFor(cf => cf.FollowingId)
            .NotNull()
            .WithMessage("FollowingId cannot be null.")
            .GreaterThan(0)
            .WithMessage("FollowingId must be greater than zero.");
    }
}