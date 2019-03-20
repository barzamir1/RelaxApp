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
                //apikey must be private
                string apiKey = "apiKey"; //replace with real apiKey
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("relaxAppNotifications@gmail.com", "relaxAppNotifications"),
                    // send a message to the emergency contact email
                    // msg content is that the current user (first+last name) is having a stress moment
                    Subject = "Stress Moment Alert!",
                    PlainTextContent = "Hello from relaxApp! \n",
                    HtmlContent = string.Format("<p dir=ltr><strong>Hello {0}!</strong><br /><br />{1} {2} is having a stress moment<br />Why don&#39;t you reach out?<br /><br /><br /> RelaxApp Team </p>", Login.Default.CurrentUser.EmergencyContactName, Login.Default.CurrentUser.FirstName, Login.Default.CurrentUser.LastName)
                };
                msg.AddTo(new EmailAddress(Login.Default.CurrentUser.EmergencyContactEmail, Login.Default.CurrentUser.EmergencyContactName));

                // send message without waiting
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            }
        }
    }
}
//&SG&.&Zrm7iBs9R&h2&BheMMJs8csA.fmnn_V96&OTPZn4_4qtt-c08YHaJ&Jbz&PrLDN&N6N9&78Lc& //remove all '&'
