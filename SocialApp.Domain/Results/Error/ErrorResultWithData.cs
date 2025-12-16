using System.Net;
using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Error;

public class ErrorResultWithData<T> : IServiceResultWithData<T> where T : class
{
    public bool Success => false;
    public string Message { get; }
    public int StatusCode { get; }
    public T? Data { get; }

    public ErrorResultWithData(string message, int statusCode = (int)HttpStatusCode.BadRequest)
    {
        Message = message;
        StatusCode = statusCode;
    }
}
