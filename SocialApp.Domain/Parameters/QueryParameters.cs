namespace SocialApp.Domain.Parameters;

public class QueryParameters
{
    public string? Search { get; set; }
    public string Include { get; set; } = "";
}