using FluentValidation;
using SocialApp.Domain.DTOs;

namespace SocialApp.Application.Validators.DTO.Update;

public class UpdateProfileDTOValidator : AbstractValidator<UpdateProfileDTO>
{
    public UpdateProfileDTOValidator()
    {
        RuleFor(dto => dto.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

        RuleFor(dto => dto.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(dto => dto.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(400).WithMessage("Bio maximum can be 400 characters.");
        
        RuleFor(x => x.Location)
            .MaximumLength(70).WithMessage("Location maximum can be 70 characters.");
        
        RuleFor(x => x.Website)
            .MaximumLength(100).WithMessage("Website maximum can be 100 characters.");
    }
}