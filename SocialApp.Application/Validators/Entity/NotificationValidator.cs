using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class NotificationValidator : AbstractValidator<Notification>
{
    public NotificationValidator()
    {
        RuleFor(n => n.Message)
            .NotEmpty()
            .WithMessage("Message field cannot be empty.")
            .MaximumLength(500)
            .WithMessage("Message field must be maximum 500 characters.");
        
        RuleFor(n => n.UserId)
            .NotNull()
            .WithMessage("UserId cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
    }
}