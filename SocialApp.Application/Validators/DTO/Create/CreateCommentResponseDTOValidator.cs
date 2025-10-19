using FluentValidation;
using SocialApp.Domain.DTOs.Create;

namespace SocialApp.Application.Validators.DTO.Create;

public class CreateCommentResponseDTOValidator : AbstractValidator<CreateCommentResponseDTO>
{
    public CreateCommentResponseDTOValidator()
    {
        RuleFor(ccr => ccr.CommentId)
            .NotNull()
            .WithMessage("CommentId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("CommentId value must be greater than zero.");
    }
}