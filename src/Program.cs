using LupuServ;

using Microsoft.Extensions.Options;

using Serilog;

using SmtpServer;
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

builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();