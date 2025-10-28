using Microsoft.AspNetCore.Builder;
using SocialApp.Application.Middlewares;

namespace SocialApp.Application.Registrations;

public static class MiddlewareExtensionRegistration
{
    public static IApplicationBuilder UseGlobalException(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionMiddleware>();
}