using LupuServ;

using Microsoft.Extensions.Options;

using Polly;

using Serilog;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(config =>
{
    config.ClearProviders();

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(applyThemeToRedirectedOutput: true)
        .CreateBootstrapLogger();

    config.AddSerilog(Log.Logger);
});

IConfigurationSection config = builder.Configuration.GetSection("Service");

builder.Services.Configure<ServiceConfig>(config);

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

IHost host = builder.Build();
host.Run();