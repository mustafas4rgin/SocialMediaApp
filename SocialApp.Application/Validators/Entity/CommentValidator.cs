using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(c => c.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(c => c.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");

        RuleFor(c => c.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.")
            .Length(4, 128)
            .WithMessage("Body length must be between 4-128 characters.");
    }
}