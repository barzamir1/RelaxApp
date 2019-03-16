using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BreathePage : ContentPage
	{

        public BreathePage ()
		{
            InitializeComponent();

            webView.Source = "https://www.calm.com/breathe";
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await Task.Delay(60 * 1000);
                await Navigation.PopAsync();
            }
            catch { }

        }

    }
}