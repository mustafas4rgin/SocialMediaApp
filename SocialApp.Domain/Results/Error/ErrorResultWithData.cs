
using SocialApp.Domain.Contracts;

namespace SocialApp.Domain.Results.Error;

public class ErrorResultWithData<T> : ServiceResultWithData<T>, IServiceResultWithData<T>  where T : class
{
    public ErrorResultWithData(string message) : base(message, default!, false) {}
}