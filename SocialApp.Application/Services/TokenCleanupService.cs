using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(3));

        do
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IAuthRepository>();

                var expiredTokens = await repo.GetExpiredAccessTokensAsync(stoppingToken);

                if (expiredTokens.Count > 0)
                {
                    foreach (var token in expiredTokens)
                        repo.Delete(token);

                    await repo.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Deleted {Count} expired access tokens.", expiredTokens.Count);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token cleanup failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
