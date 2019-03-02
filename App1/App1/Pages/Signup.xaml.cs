using App1.DataObjects;
using App1.Pages;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Signup : ContentPage
    {
        Users user;
        private MobileServiceClient _mobileServiceClient;
        IMobileServiceSyncTable<Users> UsersTable;
        public Signup()
        {
            InitializeComponent();
            user = new Users();
            user.id = Login.Default.CurrentUser.id;
            _mobileServiceClient = Services.AzureDataService.Instance._mobileServiceClient;
            UsersTable = _mobileServiceClient.GetSyncTable<Users>();
            UsersTable.PullAsync("Users", UsersTable.CreateQuery());
            isTherapistPicker.SelectedIndex = 0;
        }

        private async void Signup_Clicked(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;
            user.FirstName = firstName.Text;
            user.LastName = lastName.Text;
            user.DateOfBirth = dobPicker.Date;
            user.Gender = genderPicker.SelectedItem.ToString();
            if (isTherapistPicker.SelectedIndex == 0) //user
            {
                user.Occupation = occupation.Text;
                user.EmergencyContactName = emergencyContactName.Text;
                user.EmergencyContactPhone = emergencyContactPhone.Text;
                user.EmergencyContactEmail = emergencyContactEmail.Text;
                user.isTherapist = false;
            }
            else //therapist
            {
                user.Occupation = "therapist";
                user.isTherapist = true;
            }
            user.shortID = RandomShortUserID();

            await UsersTable.InsertAsync(user);
            await _mobileServiceClient.SyncContext.PushAsync();
            Login.Default.CurrentUser = user;
            if (user.isTherapist)
                await Navigation.PushAsync(new TherapistPage());
            else 
                await Navigation.PushAsync(new SignupRelaxTest()); //navigate to test me relaxed

            Navigation.RemovePage(this);
        }

        private bool ValidateInput()
        {
            if (firstName.Text == null || !isAlphabetic(firstName.Text))
                return false;
            if (lastName.Text == null || !isAlphabetic(lastName.Text))
                return false;
            if (dobPicker.Date.CompareTo(DateTime.Now) > 0) //birthday is later than today
                return false;
            if (genderPicker.SelectedIndex < 0)
                return false;
            if (isTherapistPicker.SelectedIndex > 0)
                return true;
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
            // validate email address - allow empty field
            if (emergencyContactEmail.Text != null && emergencyContactEmail.Text != "")
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

        private void IsTherapistPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isTherapistPicker.SelectedIndex == 0) //user
            {
                UserExtraDetails.IsVisible = true;
            }
            else //therapist
            {
                UserExtraDetails.IsVisible = false;
            }
        }
        private String RandomShortUserID()
        {
            int size = 8;
            String chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            Random rand = new Random();
            char[] selectedChars = new char[size];
            for(int i=0; i<size; i++)
            {
                selectedChars[i] = chars[rand.Next(chars.Length)];
            }
            return new string(selectedChars);
        }

        private void Name_Completed(object sender, EventArgs e)
        {
            if (sender == firstName)
                lastName.Focus();
        }
    }
}