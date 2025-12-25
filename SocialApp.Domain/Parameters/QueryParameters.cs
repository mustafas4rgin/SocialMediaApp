namespace SocialApp.Domain.Parameters;

public class QueryParameters
{
    public string? Search { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
}
