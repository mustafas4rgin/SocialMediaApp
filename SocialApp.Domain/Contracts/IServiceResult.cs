namespace SocialApp.Domain.Contracts;

public interface IServiceResult
{
    bool Success { get; }
    string Message { get; }
    int StatusCode { get; }
}
