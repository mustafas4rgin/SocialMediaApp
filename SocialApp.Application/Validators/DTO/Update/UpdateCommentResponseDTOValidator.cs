using FluentValidation;
using SocialApp.Domain.DTOs.Update;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdateCommentResponseDTOValidator : AbstractValidator<UpdateCommentResponseDTO>
{
    public UpdateCommentResponseDTOValidator()
    {
        RuleFor(ucr => ucr.CommentId)
            .NotNull()
            .WithMessage("CommentId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("CommentId value must be greater than zero.");
    }
}