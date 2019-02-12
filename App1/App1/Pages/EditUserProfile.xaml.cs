using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using App1.DataObjects;
using App1.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditUserProfile : ContentPage
    {
        Users user;

        public EditUserProfile()
        {
            InitializeComponent();
            user = Login.Default.CurrentUser;
            firstName.Text = Login.Default.CurrentUser.FirstName;
            lastName.Text = Login.Default.CurrentUser.LastName;
            occupation.Text = Login.Default.CurrentUser.Occupation;
            emergencyContactName.Text = Login.Default.CurrentUser.EmergencyContactName;
            emergencyContactPhone.Text = Login.Default.CurrentUser.EmergencyContactPhone;
            emergencyContactEmail.Text = Login.Default.CurrentUser.EmergencyContactEmail;
        }

        private async void save_changes_clicked(object sender, EventArgs e)
        {
            // update user details
            var azure = AzureDataService.Instance;
            if (!ValidateInput())
            {
                await DisplayAlert("Invalid Data", "Make sure all of your details are correct", "OK");
                return;
            }
            user.FirstName = firstName.Text;
            user.LastName = lastName.Text;
            user.Occupation = occupation.Text;
            user.EmergencyContactName = emergencyContactName.Text;
            user.EmergencyContactPhone = emergencyContactPhone.Text;
            user.EmergencyContactEmail = emergencyContactEmail.Text;
            // sync changes from local db to azure
            azure.UpdateUser(user);
            // popup - changes are saved
            await DisplayAlert("Deatils Updated!", "", "OK");
            // go back to AppToc page
            await Navigation.PushAsync(new App1.Page1());
        }

        private bool ValidateInput()
        {
            if (firstName.Text == null || !isAlphabetic(firstName.Text))
                return false;
            if (lastName.Text == null || !isAlphabetic(lastName.Text))
                return false;
            if (occupation.Text == null || !isAlphabetic(occupation.Text))
                return false;
            if (emergencyContactName.Text != null)
            {
                if (!isAlphabetic(emergencyContactName.Text))
                    return false;
                if (emergencyContactPhone.Text == null)
                    return false;
                if (!isNumeric(emergencyContactPhone.Text))
                    return false;
            }
            // validate email address & allow empty field
            if (emergencyContactEmail.Text != null  && emergencyContactEmail.Text != "")
            {
                if (!isValidEmail(emergencyContactEmail.Text))
                    return false;
            }

            return true;

        }
        private bool isAlphabetic(String text)
        {
            Regex pattern = new Regex("^[a-zA-Z ]+$");
            bool res = pattern.IsMatch(text);
            return res;
        }
        private bool isNumeric(String text)
        {
            Regex pattern = new Regex("[0-9]");
            bool res = pattern.IsMatch(text);
            return res;
        }

        private bool isValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }


}

