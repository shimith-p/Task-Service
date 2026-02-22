using Microsoft.AspNetCore.Mvc;
using TaskService.Api.Extensions;
using TaskService.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Services.Configure<ApiBehaviorOptions>(opts =>
{
    opts.SuppressModelStateInvalidFilter = true;
});

builder.Services.RegisterApplicationServices();

builder.Services.AddControllers();
 
builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

builder.Services.RegisterDbContextExtensions(builder.Configuration);

builder.Services.RegisterSwaggerExtensions();

builder.Services.RegisterCorsExtensions();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program
{
}

