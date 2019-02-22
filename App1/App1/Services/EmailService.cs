using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class EmailService
    {
        public static async Task Execute()
        {
            if (Login.Default.CurrentUser == null)
                return;
            // send mail to EmergencyContactEmail
            if (Login.Default.CurrentUser.EmergencyContactEmail != null && Login.Default.CurrentUser.EmergencyContactEmail != "")
            {
                //var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                string apiKey = "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";
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("relaxAppNotifications@gmail.com", "relaxAppNotifications"),
                    // send a message to the emergency contact email
                    // msg content is that the current user (first+last name) is having a stress moment
                    Subject = "Stress Moment Alert!",
                    PlainTextContent = "Hello from relaxApp! \n",
                    HtmlContent = string.Format("<p dir=ltr><strong>Hello {0}!</strong><br /><br />{1} {2} is having a stress moment<br />Why don &#39;t you give&nbsp;a call?<br /><br /> RelaxApp Team </p>", Login.Default.CurrentUser.EmergencyContactName, Login.Default.CurrentUser.FirstName, Login.Default.CurrentUser.LastName)
                };
                msg.AddTo(new EmailAddress(Login.Default.CurrentUser.EmergencyContactEmail, Login.Default.CurrentUser.EmergencyContactName));
                // send message without waiting
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            }
        }
    }
}
