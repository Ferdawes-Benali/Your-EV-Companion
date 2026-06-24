using SmartEVCompanion.Application.DTOs;

namespace SmartEVCompanion.Application.Services
{
    /// <summary>
    /// Factory for creating pre-formatted email messages.
    /// Contains static methods for different email types.
    /// </summary>
    public static class EmailMessageFactory
    {
        private const string FromEmail = "noreply@evchargers.tn";

        /// <summary>
        /// Creates a welcome email for new users.
        /// Bilingual content in French and Arabic.
        /// </summary>
        /// <param name="toEmail">Recipient's email address.</param>
        /// <param name="displayName">User's display name.</param>
        /// <returns>Formatted welcome email message.</returns>
        public static EmailMessage CreateWelcome(string toEmail, string displayName)
        {
            var htmlBody = $@"<!DOCTYPE html>
<html dir=""ltr"" lang=""fr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2ecc71; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ margin: 20px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 30px; }}
        .ar-text {{ direction: rtl; text-align: right; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🔌 Bienvenue sur EV Chargers Tunisia!</h1>
        </div>
        <div class=""content"">
            <p>Bonjour <strong>{displayName}</strong>,</p>
            <p>Merci de vous être inscrit(e) sur EV Chargers Tunisia, votre application pour trouver les stations de recharge pour véhicules électriques en Tunisie.</p>
            <p>Avec notre plateforme, vous pouvez :</p>
            <ul>
                <li>Localiser les stations de recharge à proximité</li>
                <li>Estimer l'autonomie de votre véhicule</li>
                <li>Consulter les avis des autres utilisateurs</li>
                <li>Trouver les horaires d'ouverture et services</li>
            </ul>
            <p>Commencez dès maintenant à explorer les stations disponibles!</p>
        </div>
        <div class=""ar-text"">
            <h3>أهلا وسهلا في EV Chargers Tunisia 🔌</h3>
            <p>شكراً لك على التسجيل معنا</p>
            <p>مرحباً <strong>{displayName}</strong>،</p>
            <p>شكراً لتسجيلك في EV Chargers Tunisia، تطبيقك للعثور على محطات شحن المركبات الكهربائية في تونس.</p>
            <p>مع منصتنا، يمكنك:</p>
            <ul>
                <li>تحديد موقع محطات الشحن القريبة</li>
                <li>تقدير نطاق سيارتك</li>
                <li>قراءة آراء المستخدمين الآخرين</li>
                <li>العثور على أوقات العمل والخدمات</li>
            </ul>
            <p>ابدأ الآن في استكشاف المحطات المتاحة!</p>
        </div>
        <div class=""footer"">
            <p>© 2026 EV Chargers Tunisia. Tous droits réservés.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage(
                To: toEmail,
                Subject: "Bienvenue sur EV Chargers Tunisia 🔌",
                HtmlBody: htmlBody,
                Type: EmailType.Welcome
            );
        }

        /// <summary>
        /// Creates a review confirmation email.
        /// Bilingual content in French and Arabic.
        /// </summary>
        /// <param name="toEmail">Recipient's email address.</param>
        /// <param name="stationName">Name of the station reviewed.</param>
        /// <returns>Formatted review confirmation email message.</returns>
        public static EmailMessage CreateReviewConfirmation(string toEmail, string stationName)
        {
            var htmlBody = $@"<!DOCTYPE html>
<html dir=""ltr"" lang=""fr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #3498db; color: white; padding: 20px; text-align: center; border-radius: 5px; }}
        .content {{ margin: 20px 0; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 30px; }}
        .ar-text {{ direction: rtl; text-align: right; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; }}
        .highlight {{ background-color: #ecf0f1; padding: 10px; border-radius: 5px; margin: 10px 0; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>✅ Votre avis a été publié!</h1>
        </div>
        <div class=""content"">
            <p>Merci beaucoup pour votre contribution!</p>
            <p>Votre avis pour la station <strong>{stationName}</strong> a été publié avec succès et est maintenant visible aux autres utilisateurs.</p>
            <div class=""highlight"">
                <p><strong>Votre avis aide la communauté!</strong></p>
                <p>Vos retours contribuent à améliorer l'expérience de tous les utilisateurs de EV Chargers Tunisia.</p>
            </div>
            <p>Merci de votre engagement envers notre communauté de véhicules électriques.</p>
        </div>
        <div class=""ar-text"">
            <h3>✅ تم نشر تقييمك!</h3>
            <p>شكراً لك على مساهمتك!</p>
            <p>تم نشر تقييمك لمحطة <strong>{stationName}</strong> بنجاح وهو الآن مرئي للمستخدمين الآخرين.</p>
            <div class=""highlight"">
                <p><strong>تقييمك يساعد المجتمع!</strong></p>
                <p>ملاحظاتك تساهم في تحسين تجربة جميع مستخدمي EV Chargers Tunisia.</p>
            </div>
            <p>شكراً لك على التزامك تجاه مجتمعنا للمركبات الكهربائية.</p>
        </div>
        <div class=""footer"">
            <p>© 2026 EV Chargers Tunisia. جميع الحقوق محفوظة.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage(
                To: toEmail,
                Subject: "Votre avis a été publié",
                HtmlBody: htmlBody,
                Type: EmailType.ReviewConfirmation
            );
        }
    }
}
