using SmartEVCompanion.Application.DTOs;

namespace SmartEVCompanion.Application.Common.Interfaces
{
    /// <summary>
    /// Decouples email sending from request processing using an in-process queue.
    /// Emails are enqueued here and dequeued asynchronously by a background service.
    /// </summary>
    public interface IEmailQueue
    {
        /// <summary>
        /// Enqueues an email message for sending.
        /// This is a non-blocking, fire-and-forget operation.
        /// </summary>
        /// <param name="message">The email message to send.</param>
        void Enqueue(EmailMessage message);
    }
}
