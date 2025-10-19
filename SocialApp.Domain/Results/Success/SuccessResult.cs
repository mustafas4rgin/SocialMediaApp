using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Success;

public class SuccessResult : ServiceResult, IServiceResult
{
    public SuccessResult(string message) : base(true, message) {}
}