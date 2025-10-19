using FluentValidation;
using SocialApp.Domain.DTOs.Create;

namespace SocialApp.Application.Validators.DTO.Create;

public class CreateCommentDTOValidator : AbstractValidator<CreateCommentDTO>
{
    public CreateCommentDTOValidator()
    {
        RuleFor(cc => cc.PostId)
            .NotNull()
            .WithMessage("PostId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("PostId value must be greater than zero.");

        RuleFor(cc => cc.UserId)
            .NotNull()
            .WithMessage("UserId value cannot be null.")
            .GreaterThan(0)
            .WithMessage("UserId value must be greater than zero.");
        
        RuleFor(cc => cc.Body)
            .NotEmpty()
            .WithMessage("Body cannot be empty.")
            .Length(4, 128)
            .WithMessage("Body length must be between 4-128 characters.");
    }
}