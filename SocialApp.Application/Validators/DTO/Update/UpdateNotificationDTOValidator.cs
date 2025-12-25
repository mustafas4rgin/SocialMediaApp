using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Create;

public class UpdateNotificationDTOValidator : AbstractValidator<UpdateNotificationDTO>
{
    public UpdateNotificationDTOValidator()
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