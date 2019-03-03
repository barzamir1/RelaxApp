using App1.ViewModels;
using System;
using WorkingWithMaps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatsTabbedPage : TabbedPage
    {
        public StatsTabbedPage()
        {
            InitializeComponent();
            InitAllPages();
        }

        private async void InitAllPages()
        {
            Title = "Statistics";
            var loading = new LoadingAnimation();
            Children.Add(loading);

            //make sure all tables are loaded when creating the following pages
            var model = await MeasurementsPageViewModel.GetInstance();
            await model.InitializeActivities();
            await model.InitializeMeasurement();

            Children.Add(new CalendarStats() { Title = "Calendar", Icon = "calendar.png" });
            Children.Add(new PinPage() { Title = "Map", Icon = "map.png" });
            //TODO: consider removing these pages:
            Children.Add(new LastMeasurementsListPage() { Title = "All", Icon = "list.png" });
            //Children.Add(new ActivitiesListPage() { Title = "Activities", Icon = "activity.png" });

            loading.Complete();
            Children.Remove(loading);
            this.CurrentPageChanged += CurrentTabChanged;
        }
        private void CurrentTabChanged(object sender, EventArgs args)
        {
            if(this.CurrentPage is PinPage)
            {
                ((PinPage)this.CurrentPage).SetPins();
            }
        }
    }
}