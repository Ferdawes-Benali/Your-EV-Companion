using SmartEVCompanion.Application.Common.Interfaces;
using SmartEVCompanion.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SmartEVCompanion.Infrastructure.Email
{
    /// <summary>
    /// In-process email queue using System.Threading.Channels.
    /// Decouples email sending from request processing.
    /// Implements both IEmailQueue and provides async enumeration for background service.
    /// </summary>
    public class InProcessEmailQueue : IEmailQueue
    {
        private readonly Channel<EmailMessage> _channel;

        public InProcessEmailQueue()
        {
            _channel = Channel.CreateUnbounded<EmailMessage>();
        }

        /// <summary>
        /// Enqueues an email message for processing by the background service.
        /// Non-blocking operation.
        /// </summary>
        public void Enqueue(EmailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!_channel.Writer.TryWrite(message))
            {
                throw new InvalidOperationException("Failed to enqueue email message");
            }
        }

        /// <summary>
        /// Asynchronously reads all queued email messages.
        /// Used by EmailDispatcherBackgroundService to process emails.
        /// </summary>
        public async IAsyncEnumerable<EmailMessage> ReadAllAsync([EnumeratorCancellation] CancellationToken ct)
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(ct))
            {
                yield return message;
            }
        }
    }
}
