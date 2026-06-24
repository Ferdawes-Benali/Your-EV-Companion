using SmartEVCompanion.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace SmartEVCompanion.Application.Common.Interfaces
{
    /// <summary>
    /// Sends email messages via an external email service provider.
    /// Implementations should handle retries and logging gracefully.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email message.
        /// 
        /// Should not throw exceptions; failures are logged and handled gracefully.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        /// <param name="ct">Cancellation token.</param>
        Task SendAsync(EmailMessage message, CancellationToken ct);
    }
}
