using DieselTimeDeliveries;
using DieselTimeDeliveries.ServiceDefaults;
using Marten;
using Oakton;
using Oakton.Resources;
using Routing;
using Routing.Application;
using Warehouse;
using Warehouse.Application;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;
using Wolverine.Postgresql;

var builder = WebApplication.CreateBuilder(args);
const string serviceName = "DieselTimeDeliveries";

builder.AddServiceDefaults(serviceName);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWolverineHttp();
builder.Services.AddControllers();
builder.Host.ApplyOaktonExtensions();

var martenString = builder.Configuration.GetConnectionString(EnvConstants.MartenConnectionString);
if (martenString == null)
{
    throw new Exception("marten string is null");
}
builder.Services.AddMarten(opts =>
{
    opts.Connection(martenString);
    opts.DisableNpgsqlLogging = true;
    opts.DatabaseSchemaName = "chaos2";
}).IntegrateWithWolverine();

builder.Host.UseResourceSetupOnStartup();
builder.Host.UseWolverine(opts =>
{
    // Surely plenty of other configuration for Wolverine...

    // This *temporary* line of code will write out a full report about why or
    // why not Wolverine is finding this handler and its candidate handler messages
    Console.WriteLine(opts.DescribeHandlerMatch(typeof(RoutingHandler)));

    opts.Policies.UseDurableLocalQueues();
    opts.Policies.UseDurableInboxOnAllListeners();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    opts.DefaultLocalQueue.UseDurableInbox();
    opts.ServiceName = serviceName;
    
    opts.Policies.MessageSuccessLogLevel(LogLevel.Debug);
    opts.Policies.LogMessageStarting(LogLevel.Information);
    opts.Policies.MessageExecutionLogLevel(LogLevel.Information);
});

var dbConnectionString = builder.Configuration.GetConnectionString(EnvConstants.DbConnectionString) ?? throw new Exception("Invalid connection string for postgres");
Console.WriteLine(dbConnectionString);
Console.WriteLine("--------------------------");
builder.Services.WarehouseInstall(dbConnectionString);

var googleApiKey = builder.Configuration.GetConnectionString(EnvConstants.GoogleApiKey) ?? throw new Exception("Invalid google api key");
builder.Services.RoutingInstall(googleApiKey);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSwagger();

app.MapControllers();
app.MapWolverineEndpoints();
app.MapDefaultEndpoints();

app.Run();