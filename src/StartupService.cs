using Coravel.Queuing.Interfaces;

using LupuServ.Invocables;

namespace LupuServ;

/// <summary>
///     Performs startup tasks and runs SMTP listener.
/// </summary>
public class StartupService : BackgroundService
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<StartupService> _logger;
    private readonly IQueue _queue;
    private readonly SmtpServer.SmtpServer _smtpServer;

    public StartupService(ILogger<StartupService> logger, SmtpServer.SmtpServer smtpServer, IHostEnvironment environment,
        IQueue queue)
    {
        _logger = logger;
        _smtpServer = smtpServer;
        _environment = environment;
        _queue = queue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // for tests and experiments
        if (_environment.IsDevelopment())
        {
            // fetch sensor status
            _queue.QueueInvocable<GetSensorListInvocable>();

            return;
        }
        
        _logger.LogInformation("Fetching sensor status");
        
        _queue.QueueInvocable<GetSensorListInvocable>();

        _logger.LogInformation("Starting SMTP server");

        await _smtpServer.StartAsync(stoppingToken);
    }
}