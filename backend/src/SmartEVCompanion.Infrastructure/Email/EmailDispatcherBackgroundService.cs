using SmartEVCompanion.Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Infrastructure.Email
{
    /// <summary>
    /// Background service that continuously processes emails from the in-process queue.
    /// Runs for the lifetime of the application and dequeues messages to send.
    /// </summary>
    public class EmailDispatcherBackgroundService : BackgroundService
    {
        private readonly InProcessEmailQueue _emailQueue;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<EmailDispatcherBackgroundService> _logger;

        public EmailDispatcherBackgroundService(
            InProcessEmailQueue emailQueue,
            IEmailSender emailSender,
            ILogger<EmailDispatcherBackgroundService> logger)
        {
            _emailQueue = emailQueue ?? throw new ArgumentNullException(nameof(emailQueue));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Continuously reads from the email queue and sends emails.
        /// Runs until the application is shutting down.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Email dispatcher background service started");

            try
            {
                // Process emails as they are enqueued
                await foreach (var emailMessage in _emailQueue.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        _logger.LogDebug("Processing email to {To} ({Type})", emailMessage.To, emailMessage.Type);
                        await _emailSender.SendAsync(emailMessage, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Email sending was cancelled");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing email to {To}", emailMessage.To);
                        // Continue processing other emails even if one fails
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Email dispatcher background service is shutting down");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in email dispatcher background service");
            }
        }
    }
}
