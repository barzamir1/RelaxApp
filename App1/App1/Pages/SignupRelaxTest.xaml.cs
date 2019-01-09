using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SignupRelaxTest : ContentPage
	{
		public SignupRelaxTest ()
		{
			InitializeComponent ();
            initBand();
		}

        private async void Start_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TestMe(true));
        }
        private async void initBand()
        {
            await DependencyService.Get<IBand>().ConnectToBand(new TestMeViewModel());
            DependencyService.Get<IBand>().RequestConsent();
        }
    }
}