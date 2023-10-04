using LupuServ.Services;

namespace LupuServ;

public class Worker : BackgroundService
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly SmtpServer.SmtpServer _smtpServer;

    public Worker(ILogger<Worker> logger, SmtpServer.SmtpServer smtpServer, IHostEnvironment environment,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _smtpServer = smtpServer;
        _environment = environment;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // for tests and experiments
        if (_environment.IsDevelopment())
        {
            IMessageGateway gw = _serviceProvider.GetRequiredService<IMessageGateway>();

            // TODO: finish me!

            return;
        }

        _logger.LogInformation("Starting SMTP server");

        await _smtpServer.StartAsync(stoppingToken);
    }
}