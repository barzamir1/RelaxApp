using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignupStressTest : ContentPage
	{
		public SignupStressTest ()
		{
			InitializeComponent ();
		}
        private async void Start_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
            Navigation.RemovePage(this);
        }
    }
}