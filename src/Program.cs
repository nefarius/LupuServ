﻿using Coravel;

using LupuServ;
using LupuServ.Invocables;
using LupuServ.Services;
using LupuServ.Services.Gateways;
using LupuServ.Services.Web;

using Microsoft.Extensions.Options;

using MongoDB.Driver;
using MongoDB.Entities;

using Polly;
using Polly.Contrib.WaitAndRetry;

using Refit;

using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(applyThemeToRedirectedOutput: true, theme: AnsiConsoleTheme.Literate)
        .CreateBootstrapLogger();

    config.AddSerilog(Log.Logger);
});

// Config
IConfigurationSection config = builder.Configuration.GetSection("Service");
ServiceConfig? serviceConfig = config.Get<ServiceConfig>();

if (serviceConfig is null)
{
    throw new ArgumentException("Configuration incomplete!");
}

builder.Services.Configure<ServiceConfig>(config);

// Gateways
switch (serviceConfig.Gateway)
{
    case GatewayService.CM:
        builder.Services.AddTransient<IMessageGateway, CMMessageGateway>();
        break;
    case GatewayService.ClickSend:
        builder.Services.AddTransient<IMessageGateway, ClickSendGateway>();
        break;
    default:
        throw new ArgumentOutOfRangeException(nameof(serviceConfig.Gateway), "Unknown gateway service");
}

// ClickSend API
builder.Services.AddTransient<ClickSendBasicAuthHeaderHandler>();
builder.Services.AddRefitClient<IClickSendApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://rest.clicksend.com/"))
    .AddHttpMessageHandler<ClickSendBasicAuthHeaderHandler>();
// Central Station Web APIs
builder.Services.AddTransient<CentralStationBasicAuthHeaderHandler>();
builder.Services.AddRefitClient<ISensorListApi>(new RefitSettings(new BrokenJsonSerializer()))
    .ConfigureHttpClient(c => c.BaseAddress = serviceConfig.CentralStation.Address)
    .AddHttpMessageHandler<CentralStationBasicAuthHeaderHandler>()
    .AddTransientHttpErrorPolicy(pb =>
        pb.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10)));

// Scheduler
builder.Services.AddScheduler();
builder.Services.AddQueue();
builder.Services.AddTransient<GetSensorListInvocable>();

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

// Spins up SMTP server instance
builder.Services.AddHostedService<StartupService>();

// Database
string? connectionString = builder.Configuration.GetConnectionString("MongoDB");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentException("Configuration incomplete!");
}

Log.Logger.Information("Connecting to database");

DB.InitAsync(serviceConfig.DatabaseName, MongoClientSettings.FromConnectionString(connectionString))
    .GetAwaiter()
    .GetResult();

Log.Logger.Information("Database connected");

IHost host = builder.Build();

// register scheduled jobs
host.Services.UseScheduler(scheduler =>
    {
        scheduler
            .Schedule<GetSensorListInvocable>()
            .EveryFifteenMinutes();
    }
);

host.Run();