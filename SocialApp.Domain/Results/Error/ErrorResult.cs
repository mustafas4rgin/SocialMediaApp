using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Error;

public class ErrorResult : ServiceResult, IServiceResult
{
    public ErrorResult(string message) : base(false, message) {}
}