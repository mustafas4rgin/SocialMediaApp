using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SocialApp.Application.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred at {Path}", context.Request.Path);

            var isDev = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                "Development",
                StringComparison.OrdinalIgnoreCase);

            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();
            int statusCode;

            switch (ex)
            {
                case ValidationException validationEx:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    response.Title = "Doğrulama Hatası";
                    response.Message = string.Join(" | ", validationEx.Errors.Select(e => e.ErrorMessage));
                    if (isDev) response.Message += $" | {ex.Message}";
                    break;

                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response.Title = "Yetkisiz Erişim";
                    response.Message = "Bu işlemi yapma yetkiniz yok.";
                    if (isDev) response.Message += $" | {ex.Message}";
                    break;

                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    response.Title = "Kayıt Bulunamadı";
                    response.Message = ex.Message;
                    break;

                case InvalidOperationException:
                    statusCode = (int)HttpStatusCode.Conflict;
                    response.Title = "Geçersiz İşlem";
                    response.Message = ex.Message;
                    break;

                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response.Title = "Sunucu Hatası";
                    response.Message = isDev ? ex.Message : "Beklenmeyen bir hata oluştu.";
                    break;
            }

            response.StatusCode = statusCode;
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private class ErrorResponse
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public int StatusCode { get; set; }
    }
}
