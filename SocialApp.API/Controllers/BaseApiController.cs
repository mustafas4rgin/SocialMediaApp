using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Domain.Contracts;

namespace SocialApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        //protected int? CurrentUserId => User.GetUserId();
        protected IActionResult? HandleServiceResult<T>(IServiceResultWithData<T> result) where T : class
        {
            if (!result.Success)
                return NotFound(new { result.Message });

            return null;
        }

        protected IActionResult? HandleServiceResult(IServiceResult result)
        {
            if (!result.Success)
                return BadRequest(new { result.Message });

            return null;
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
}
