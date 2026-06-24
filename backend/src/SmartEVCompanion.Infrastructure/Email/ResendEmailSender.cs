using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Infrastructure.Email
{
    /// <summary>
    /// Implements IEmailSender using Resend email service API.
    /// Endpoint: https://api.resend.com/emails
    /// Handles failures gracefully without throwing (fire-and-forget pattern).
    /// </summary>
    public class ResendEmailSender : IEmailSender
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ResendEmailSender> _logger;

        public ResendEmailSender(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<ResendEmailSender> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends an email via Resend API.
        /// Logs errors but never throws — fire-and-forget pattern.
        /// </summary>
        public async Task SendAsync(EmailMessage message, CancellationToken ct)
        {
            try
            {
                // Get API key from configuration
                var apiKey = _configuration["ResendApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("ResendApiKey not configured, email not sent to {To}", message.To);
                    return;
                }

                // Prepare request body
                var requestBody = new
                {
                    from = "noreply@evchargers.tn",
                    to = new[] { message.To },
                    subject = message.Subject,
                    html = message.HtmlBody
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Set authorization header
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // Call Resend API
                var response = await _httpClient.PostAsync(
                    "emails",
                    content,
                    ct);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {To} (Type: {Type})", message.To, message.Type);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(ct);
                    _logger.LogError(
                        "Resend API returned status {StatusCode}: {ReasonPhrase}. Error: {Error}",
                        response.StatusCode, response.ReasonPhrase, errorContent);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error sending email to {To}", message.To);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Email send to {To} timed out", message.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {To}", message.To);
            }
        }
    }
}
