using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdateFollowDTOValidator : AbstractValidator<UpdateFollowDTO>
{
    public UpdateFollowDTOValidator()
    {
        RuleFor(uf => uf.FollowerId)
            .NotNull()
            .WithMessage("FollowerId value must be greater than zero.")
            .GreaterThan(0)
            .WithMessage("FollowrId value must be greater than zero.");

        RuleFor(uf => uf.FollowingId)
            .NotNull()
            .WithMessage("FollowingId cannot be null.")
            .GreaterThan(0)
            .WithMessage("FollowingId must be greater than zero.");
    }
}