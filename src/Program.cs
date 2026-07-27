using System.Net;

using Coravel;

using LupuServ;
using LupuServ.Invocables;
using LupuServ.Services;
using LupuServ.Services.Gateways;
using LupuServ.Services.Interfaces;
using LupuServ.Services.Web;

using Microsoft.Extensions.Options;

using Npgsql;

using Polly;
using Polly.Contrib.WaitAndRetry;

using Refit;

using Serilog;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
#if DEBUG
        .MinimumLevel.Debug()
#endif
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
    .AddPolicyHandler(Policy<HttpResponseMessage>
        .HandleResult(msg =>
            msg.StatusCode == HttpStatusCode.Unauthorized || // explicitly retry on 401
            ((int)msg.StatusCode >= 500 && (int)msg.StatusCode <= 599)) // transient 5xx errors
        .WaitAndRetryAsync(
            3,
            retryAttempt => Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10).ToArray()[retryAttempt],
            (response, timespan, retryCount, context) =>
            {
                Log.Logger.Warning("Retry {RetryCount} after {ResultStatusCode}", retryCount, response.Result.StatusCode);
            }));
// Gotify APIs
GotifyConfig? gotifyConfig = serviceConfig.Gotify;
if (gotifyConfig is not null)
{
    if (gotifyConfig.Status is not null && gotifyConfig.Status!.IsEnabled)
    {
        builder.Services.AddRefitClient<IGotifyStatusApi>().ConfigureHttpClient(c =>
        {
            c.BaseAddress = gotifyConfig.Status.Url ?? gotifyConfig.Url;
            c.DefaultRequestHeaders.Add("X-Gotify-Key", gotifyConfig.Status.AppToken);
        }).AddTransientHttpErrorPolicy(pb =>
            pb.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10)));
    }

    if (gotifyConfig.Alarm is not null && gotifyConfig.Alarm!.IsEnabled)
    {
        builder.Services.AddRefitClient<IGotifyAlarmApi>().ConfigureHttpClient(c =>
        {
            c.BaseAddress = gotifyConfig.Alarm.Url ?? gotifyConfig.Url;
            c.DefaultRequestHeaders.Add("X-Gotify-Key", gotifyConfig.Alarm.AppToken);
        }).AddTransientHttpErrorPolicy(pb =>
            pb.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10)));
    }

    if (gotifyConfig.System is not null && gotifyConfig.System!.IsEnabled)
    {
        builder.Services.AddRefitClient<IGotifySystemApi>().ConfigureHttpClient(c =>
        {
            c.BaseAddress = gotifyConfig.System.Url ?? gotifyConfig.Url;
            c.DefaultRequestHeaders.Add("X-Gotify-Key", gotifyConfig.System.AppToken);
        }).AddTransientHttpErrorPolicy(pb =>
            pb.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10)));
    }

    if (gotifyConfig.Sensors is not null && gotifyConfig.Sensors!.IsEnabled)
    {
        builder.Services.AddRefitClient<IGotifySensorsApi>().ConfigureHttpClient(c =>
        {
            c.BaseAddress = gotifyConfig.Sensors.Url ?? gotifyConfig.Url;
            c.DefaultRequestHeaders.Add("X-Gotify-Key", gotifyConfig.Sensors.AppToken);
        }).AddTransientHttpErrorPolicy(pb =>
            pb.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10)));
    }
}

// Scheduler / queue
builder.Services.AddScheduler();
builder.Services.AddQueue();
builder.Services.AddTransient<GetSensorListInvocable>();
builder.Services.AddTransient<StoreEventInvocable>();

// SMTP
builder.Services.AddTransient<IMessageStore, LupusMessageStore>();
builder.Services.AddSingleton(provider =>
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
string? connectionString = builder.Configuration.GetConnectionString("Events");

if (string.IsNullOrEmpty(connectionString))
{
    throw new ArgumentException("Configuration incomplete!");
}

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
builder.Services.AddSingleton<IEventStore, PostgresEventStore>();

IHost host = builder.Build();

host.Services
    .ConfigureQueue()
    .OnError(ex => Log.Logger.Error(ex, "Failed to persist queued event"));

// Ensure schema (retry for freshly started or remote Postgres)
Log.Logger.Information("Connecting to database");

IEventStore eventStore = host.Services.GetRequiredService<IEventStore>();
Policy
    .Handle<Exception>()
    .WaitAndRetry(
        10,
        retryAttempt => TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, retryAttempt))),
        (ex, delay, attempt, _) =>
        {
            Log.Logger.Warning(ex, "Database not ready (attempt {Attempt}), retrying in {Delay}", attempt, delay);
        })
    .Execute(() => eventStore.EnsureSchemaAsync().GetAwaiter().GetResult());

Log.Logger.Information("Database connected");

// register scheduled jobs
host.Services.UseScheduler(scheduler =>
    {
        scheduler
            .Schedule<GetSensorListInvocable>()
            .Daily();
    }
);

host.Run();