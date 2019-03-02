
using Plugin.Messaging;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CalmMeDownToc : ContentPage
	{
		public CalmMeDownToc ()
        { 
            InitializeComponent();
            callButton.Text = "Call " + Login.Default.CurrentUser.EmergencyContactName;
        }

        public async void openAnimalGifs(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new Pages.AnimalGifs());
        }

        public void callFreind(object sender, EventArgs args) {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall(Login.Default.CurrentUser.EmergencyContactPhone);
        }


        public async void youtubeClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new Pages.YoutubePage());
        }



    }
}