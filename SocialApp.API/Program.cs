using SocialApp.Application.Registrations;
using SocialApp.Data.Registrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddBusinessService();
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();

