using System.IO.Compression;
using FluentValidation;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Validators;

public class CommentResponseValidator : AbstractValidator<CommentResponse>
{
    public CommentResponseValidator()
    {
        RuleFor(cr => cr.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(cr => cr.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");

        RuleFor(cr => cr.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.")
            .Length(4, 128)
            .WithMessage("Body length must be between 4-128 characters.");

        RuleFor(cr => cr.CommentId)
            .NotNull()
            .WithMessage("CommentId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("CommentId value must be greater than zero.");
    }
}