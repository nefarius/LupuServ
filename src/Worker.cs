using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SmtpServer;
using SmtpServer.ComponentModel;

namespace LupuServ;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            int port = int.Parse(config.GetSection("Port").Value);

            ServiceProvider serviceProvider = new();
            serviceProvider.Add(new LupusMessageStore(config, _logger));

            ISmtpServerOptions options = new SmtpServerOptionsBuilder()
                .ServerName("localhost")
                .Port(port)
                .Build();

            _logger.LogInformation("Starting SMTP server on port {Port}", port);

            SmtpServer.SmtpServer smtpServer = new(options, serviceProvider);

            await smtpServer.StartAsync(stoppingToken);
        }
    }
}