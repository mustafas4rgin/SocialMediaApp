namespace SocialApp.Domain.Contracts;

public interface IServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
}