using App1.Pages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsPageToc : ContentPage
    {
        public StatsPageToc()
        {
            InitializeComponent();
        }

        public async void calanderClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new StatsTabbedPage());
        }

        public async void mapClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new WorkingWithMaps.PinPage());
        }

        public async void activitiesClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new ActivitiesListPage());
        }

        public async void lastMeasurementsClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new LastMeasurementsListPage());
        }
    }
}