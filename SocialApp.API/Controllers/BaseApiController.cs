using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Application.Helpers;
using SocialApp.Domain.Contracts;

namespace SocialApp.API.Controllers;

[ApiController]
public class BaseApiController : ControllerBase
{
    protected int? CurrentUserId => User.GetUserId();

    protected IActionResult? HandleServiceResult(IServiceResult result)
    {
        if (result.Success) return null;

        return StatusCode(result.StatusCode, new { result.Message });
    }

    protected IActionResult? HandleServiceResult<T>(IServiceResultWithData<T> result) where T : class
    {
        if (result.Success) return null;

        return StatusCode(result.StatusCode, new { result.Message });
    }

    protected IActionResult HandleValidationErrors(IEnumerable<ValidationFailure> errors)
    {
        var errorResponse = errors.Select(e => new
        {
            Property = e.PropertyName,
            Error = e.ErrorMessage
        });

        return BadRequest(errorResponse);
    }
}
