using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.DataObjects;

namespace App1.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmergencyContactPage : ContentPage
	{
        private String firstName = null;
        private String lastName = null;
        private String occupation = null;
        private DateTime dob;
        private String gender = null;
        private String emergencyEmail = null;
        private bool backToSignupPage = false;
        private Users user;

        public EmergencyContactPage()
        {
            InitializeComponent();
            this.backToSignupPage = true;
            showContacts_Clicked(null, null);
        }

        public EmergencyContactPage(string first, string last, string occ, DateTime dob, string gender, Users user, String mail)
        {

            InitializeComponent();

            // save the user previous changes
            this.firstName = first;
            this.lastName = last;
            this.occupation = occ;
            this.dob = dob;
            this.gender = gender;
            this.user = user;
            this.emergencyEmail = mail;

            this.backToSignupPage = true;
            showContacts_Clicked(null, null);
        }

        public EmergencyContactPage (String first, string last, string occ, String mail)
		{
			InitializeComponent ();

            // save the user previous changes
            this.firstName = first;
            this.lastName = last;
            this.occupation = occ;
            this.emergencyEmail = mail;
            showContacts_Clicked(null, null);
		}

        private async void showContacts_Clicked(object sender, EventArgs e)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    var ContactList = await DependencyService.Get<IContacts>().GetDeviceContactsAsync();
                    listContact.ItemsSource = ContactList;
                    listContact.ItemTapped += pickContact_Clicked;
                    break;
                default:
                    break;
            }
        }

        
        private async void pickContact_Clicked(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;
            var selectedItem = e.Item as ContactLists;
            Console.WriteLine(selectedItem.ContactEmail);

            // go back to the previous page after choosing contact
            if (this.backToSignupPage == false)
            {
                await Navigation.PushAsync(new EditUserProfile(firstName, lastName, occupation, selectedItem.DisplayName, selectedItem.ContactNumber, emergencyEmail));
            }
            else
            {
                MessagingCenter.Send<EmergencyContactPage, string>(this, "ContactName", selectedItem.DisplayName);
                MessagingCenter.Send<EmergencyContactPage, string>(this, "ContactPhone", selectedItem.ContactNumber);
                await Navigation.PopAsync();
                //await Navigation.PushAsync(new Signup(firstName, lastName, occupation, dob, gender, user, selectedItem.DisplayName, selectedItem.ContactNumber));
            }

            //,selectedItem.ContactEmail));

        }
    }
}
