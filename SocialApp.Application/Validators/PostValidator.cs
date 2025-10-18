using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class PostValidator : AbstractValidator<Post>
{
    public PostValidator()
    {
        RuleFor(p => p.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.")
            .Length(4, 128)
            .WithMessage("Body must be between 4-128 characters.");

        RuleFor(p => p.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
    }
}