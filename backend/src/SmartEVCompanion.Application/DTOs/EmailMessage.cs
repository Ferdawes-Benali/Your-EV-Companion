namespace SmartEVCompanion.Application.DTOs
{
    /// <summary>
    /// Represents an email message to be sent.
    /// </summary>
    public record EmailMessage(
        /// <summary>
        /// Recipient email address.
        /// </summary>
        string To,

        /// <summary>
        /// Email subject line.
        /// </summary>
        string Subject,

        /// <summary>
        /// Email body in HTML format.
        /// </summary>
        string HtmlBody,

        /// <summary>
        /// Type of email (for categorization and future templating).
        /// </summary>
        EmailType Type
    );

    /// <summary>
    /// Types of emails supported by the system.
    /// </summary>
    public enum EmailType
    {
        Welcome,
        ReviewConfirmation
    }
}
