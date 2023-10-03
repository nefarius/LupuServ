using LupuServ;
using LupuServ.Services;
using LupuServ.Services.Gateways;
using LupuServ.Services.Web;

using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Entities;

using Polly;

using Refit;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(config =>
{
    config.ClearProviders();

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(applyThemeToRedirectedOutput: true, theme: AnsiConsoleTheme.Literate)
        .CreateBootstrapLogger();

    config.AddSerilog(Log.Logger);
});

IConfigurationSection config = builder.Configuration.GetSection("Service");

builder.Services.Configure<ServiceConfig>(config);

// Gateways
builder.Services.AddTransient<IMessageGateway, CMMessageGateway>();

// Refit
builder.Services.AddTransient<AuthHeaderHandler>();
builder.Services.AddRefitClient<IClickSendApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://rest.clicksend.com/"))
    .AddHttpMessageHandler<AuthHeaderHandler>();

// SMTP
builder.Services.AddTransient<IMessageStore, LupusMessageStore>();
builder.Services.AddSingleton(
    provider =>
    {
        IOptions<ServiceConfig> cfg = provider.GetRequiredService<IOptions<ServiceConfig>>();

        ISmtpServerOptions? options = new SmtpServerOptionsBuilder()
            .ServerName("SMTP Server")
            .Port(cfg.Value.Port)
            .Build();

        return new SmtpServer.SmtpServer(options, provider.GetRequiredService<IServiceProvider>());
    });

// Singleton because there is one sender API to protect across multiple incoming messages
builder.Services.AddSingleton(Policy.RateLimitAsync<SmtpResponse>(1, TimeSpan.FromSeconds(5)));

builder.Services.AddHostedService<Worker>();

string? connectionString = builder.Configuration.GetConnectionString("MongoDB");
ServiceConfig? serviceConfig = config.Get<ServiceConfig>();

if (string.IsNullOrEmpty(connectionString) || serviceConfig is null)
{
    throw new ArgumentException("Configuration incomplete!");
}

Log.Logger.Information("Connecting to database");

DB.InitAsync(serviceConfig.DatabaseName, MongoClientSettings.FromConnectionString(connectionString))
    .GetAwaiter()
    .GetResult();

Log.Logger.Information("Database connected");

IHost host = builder.Build();
host.Run();