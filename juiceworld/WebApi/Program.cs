using System.Diagnostics;
using System.Text.Json;
using BusinessLayer.Installers;
using Commons.Constants;
using Commons.Middleware;
using JuiceWorld.Installers;
using Microsoft.AspNetCore.Diagnostics;
using WebApi.Installers;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.DalInstall();
builder.Services.BusinessLayerInstall();
builder.Services.WebApiInstall();

var app = builder.Build();

var apiPort = Environment.GetEnvironmentVariable(EnvironmentConstants.ApiPort);
if (apiPort == null)
{
    Debug.Fail(
        $"API port is null, make sure it is specified " +
        $"in the environment variable: {EnvironmentConstants.ApiPort}");
    return;
}

app.Urls.Add($"http://+{apiPort}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature != null)
        {
            var exception = exceptionHandlerPathFeature.Error;

            var result = JsonSerializer.Serialize(new { error = exception.Message });
            await context.Response.WriteAsync(result);
        }
    });
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ResponseFormatMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>("WebAPI");

// use this instead of RequestLoggingMiddleware, if compliant with the course policy
// app.UseSerilogRequestLogging();


app.MapControllers();
app.Run();
