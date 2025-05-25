using System.Diagnostics;
using BusinessLayer;
using BusinessLayer.Installers;
using Commons.Constants;
using Commons.Middleware;
using JuiceWorld.Installers;
using PresentationLayer.Mvc;
using PresentationLayer.Mvc.Installers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.BusinessLayerInstall();
builder.Services.DalInstall();
builder.Services.MvcInstall();

var app = builder.Build();

BusinessConstants.WebRootPath = app.Environment.WebRootPath;

var mvcPort = Environment.GetEnvironmentVariable(EnvironmentConstants.MvcPort);
if (mvcPort == null)
{
    Debug.Fail(
        $"MVC port is null, make sure it is specified " +
        $"in the environment variable: {EnvironmentConstants.MvcPort}");
    return;
}

app.Urls.Add($"http://+:{mvcPort}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>("MVC");

app.MapControllerRoute(
    "areas",
    "{area:exists=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
