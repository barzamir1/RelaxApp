using App1.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Page1 : ContentPage
	{
		public Page1 ()
		{
			InitializeComponent ();
            if (Login.Default.CurrentUser != null)
            {
                LabelUserName.Text = "Hello " + Login.Default.CurrentUser.FirstName;
            }
		}

        public async void openStatsPage(object sender, EventArgs args) {
            await Navigation.PushAsync(new StatsPageToc());
        }

        public async void openTestMePage(object sender, EventArgs args) {
            await Navigation.PushAsync(new MainPage());
        }

        public async void openCalmMeDownPage(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CalmMeDownToc());
        }
    }
}