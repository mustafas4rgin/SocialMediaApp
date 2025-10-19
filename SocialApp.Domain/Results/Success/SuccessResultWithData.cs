using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Success;

public class SuccessResultWithData<T> : ServiceResultWithData<T>, IServiceResultWithData<T> where T : class
{
    public SuccessResultWithData(string message, T data) : base(message, data, true) {}
}