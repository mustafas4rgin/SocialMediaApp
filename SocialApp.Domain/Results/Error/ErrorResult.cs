using System.Net;
using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Error;

public class ErrorResult : IServiceResult
{
    public bool Success => false;
    public string Message { get; }
    public int StatusCode { get; }

    public ErrorResult(string message, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        Message = message;
        StatusCode = statusCode;
    }
}
