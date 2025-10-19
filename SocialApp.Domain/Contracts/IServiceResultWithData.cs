namespace SocialApp.Domain.Contracts;

public interface IServiceResultWithData<T> where T : class
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}