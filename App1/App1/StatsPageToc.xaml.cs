using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.Pages;

namespace App1
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatsPageToc : ContentPage
	{
		public StatsPageToc ()
		{
			InitializeComponent ();
		}

        public void calanderClicked(object sender, EventArgs args) { }

        public void mapClicked(object sender, EventArgs args) { }

        public async void activitiesClicked(object sender, EventArgs args) {
            await Navigation.PushAsync(new ActivitiesListPage());
        }

        public async void lastMeasurementsClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new LastMeasurementsListPage());
        }
        

    }
}