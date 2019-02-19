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

        // constructor overloading
        public EditUserProfile(String first, String last, String occ, String emergencyName, String emergencyPhone) //, String emergencyEmail
        {
            InitializeComponent();
            user = Login.Default.CurrentUser;
            firstName.Text = first;
            lastName.Text = last;
            occupation.Text = occ;
            emergencyContactName.Text = emergencyName;
            emergencyContactPhone.Text = emergencyPhone;
            //emergencyContactEmail.Text = emergencyEmail;
        }

        

        private async void save_changes_clicked(object sender, EventArgs e)
        {
            // update user details
            var azure = AzureDataService.Instance;
            int validInputRes = ValidateInput();
            if (validInputRes > 0)
            {
                switch (validInputRes)
                {
                    case 1:
                        await DisplayAlert("Invalid First Name", "Make sure it contains only english letters", "OK");
                        return ;
                    case 2:
                        await DisplayAlert("Invalid Last Name", "Make sure it contains only english letters", "OK");
                        return;
                    case 3:
                        await DisplayAlert("Invalid Occupation", "Make sure it contains only english letters", "OK");
                        return;
                    case 4:
                        await DisplayAlert("Invalid Emergency Conatct Name", "Make sure it contains only english letters", "OK");
                        return;
                    case 5:
                        await DisplayAlert("Invalid Emergency Conatct Phone", "Make sure it contains only digits", "OK");
                        return;
                    case 6:
                        await DisplayAlert("Invalid Emergency Conatct Email", "Make sure it's a valid email address", "OK");
                        return;

                }

            }

            // if the changes are legal - save them
            user.FirstName = firstName.Text;
            user.LastName = lastName.Text;
            user.Occupation = occupation.Text;
            user.EmergencyContactName = emergencyContactName.Text;
            user.EmergencyContactPhone = emergencyContactPhone.Text;
            user.EmergencyContactEmail = emergencyContactEmail.Text;
            
            // sync changes from local db to azure
            azure.UpdateUser(user);
            
            // popup - changes are saved
            await DisplayAlert("Details Updated!", "", "OK");
            // go back to AppToc page
            await Navigation.PopToRootAsync();
        }

        private int ValidateInput()
        {
            if (firstName.Text == null || !isAlphabetic(firstName.Text))
                return 1;
            if (lastName.Text == null || !isAlphabetic(lastName.Text))
                return 2;
            if (occupation.Text == null || !isAlphabetic(occupation.Text))
                return 3;
            if (emergencyContactName.Text != null)
            {
                if (!isAlphabetic(emergencyContactName.Text))
                    return 4;
                if (emergencyContactPhone.Text == null)
                    return 5;
                if (!isNumeric(emergencyContactPhone.Text))
                    return 5;
            }
            // validate email address & allow empty field
            if (emergencyContactEmail.Text != null  && emergencyContactEmail.Text != "")
            {
                if (!isValidEmail(emergencyContactEmail.Text))
                    return 6;
            }

            return 0;

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

        // pass to pick_contact_clicked the values that the user already changed in order to save those changes
        private async void pick_contact_clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EmergencyContactPage(firstName.Text, lastName.Text, occupation.Text));
        }


    }


}

