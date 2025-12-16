using System.Net;
using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Success;

public class SuccessResultWithData<T> : IServiceResultWithData<T> where T : class
{
    public bool Success => true;
    public string Message { get; }
    public int StatusCode { get; }
    public T? Data { get; }

    public SuccessResultWithData(string message, T data, int statusCode = (int)HttpStatusCode.OK)
    {
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }
}
