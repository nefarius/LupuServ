using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmtpServer;
using SmtpServer.ComponentModel;

namespace LupuServ
{
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
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();

                var port = int.Parse(config.GetSection("Port").Value);

                var serviceProvider = new ServiceProvider();
                serviceProvider.Add(new LupusMessageStore(config, _logger));

                var options = new SmtpServerOptionsBuilder()
                    .ServerName("localhost")
                    .Port(port)
                    .Build();

                _logger.LogInformation($"Starting SMTP server on port {port}");

                var smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);

                await smtpServer.StartAsync(stoppingToken);
            }
        }
    }
}