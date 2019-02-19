using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SendGrid;
using SendGrid.Helpers.Mail;


namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class sendMail : ContentPage
    {
        public sendMail()
        {
            InitializeComponent();
            Execute();
        }

            static async Task Execute()
        {
           
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
                    HtmlContent = string.Format("<strong>Hello {0}!</strong><br /><br />{1} {2} is having a stress moment<br />Why don &#39;t you give&nbsp;a call?<br /><br /> RelaxApp Team ", Login.Default.CurrentUser.EmergencyContactName, Login.Default.CurrentUser.FirstName, Login.Default.CurrentUser.LastName)
                };
                msg.AddTo(new EmailAddress(Login.Default.CurrentUser.EmergencyContactEmail, Login.Default.CurrentUser.EmergencyContactName));
                // send message without waiting
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            }

        }
        
    }
}