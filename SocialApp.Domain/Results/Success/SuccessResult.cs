using System.Net;
using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Success;

public class SuccessResult : IServiceResult
{
    public bool Success => true;
    public string Message { get; }
    public int StatusCode { get; }

    public SuccessResult(string message, int statusCode = (int)HttpStatusCode.OK)
    {
        Message = message;
        StatusCode = statusCode;
    }
}
