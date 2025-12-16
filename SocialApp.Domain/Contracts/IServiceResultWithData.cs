namespace SocialApp.Domain.Contracts;

public interface IServiceResultWithData<T> : IServiceResult where T : class
{
    T? Data { get; }
}
