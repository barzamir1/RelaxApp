using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.Pages;

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
            _mobileServiceClient = Login.Default.ServiceClient;
            UsersTable = _mobileServiceClient.GetSyncTable<Users>();
            UsersTable.PullAsync("Users", UsersTable.CreateQuery());
        }

        private async void Signup_Clicked(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;
            user.FirstName = firstName.Text;
            user.LastName = lastName.Text;
            user.DateOfBirth = dobPicker.Date;
            user.Gender = genderPicker.SelectedItem.ToString();
            user.Occupation = occupation.Text;
            user.EmergencyContactName = emergencyContactName.Text;
            user.EmergencyContactPhone = emergencyContactPhone.Text;

            await UsersTable.InsertAsync(user);
            await _mobileServiceClient.SyncContext.PushAsync();
            Login.Default.CurrentUser = user;
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
    }
}