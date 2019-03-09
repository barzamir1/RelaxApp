using App1.Pages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
            // change to navigation bar color
            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.DarkCyan;

            if (Login.Default.CurrentUser != null)
            {
                LabelUserName.Text = "Hello " + Login.Default.CurrentUser.FirstName;
                //MeasurementsPageViewModel.GetInstance(); //start loading in background
                int repetitionTime = MeasurementHandler.measureRepetitionTime;
                DependencyService.Get<ISchedule>().ScheduleMeasurement(repetitionTime); //Schedule measurement every 6 minutes
            }
        }

        protected override void OnAppearing()
        {
            LabelUserName.Text = "Hello " + Login.Default.CurrentUser.FirstName;
        }

        public async void openStatsPage(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new StatsTabbedPage());
        }

        public async void openTestMePage(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new TestMe(false));
        }

        public async void openCalmMeDownPage(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CalmMeDownToc());
        }

        private async void gamePage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
        }

        private async void signup(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Signup());
        }
        private async void shareData(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ShareMyData());
        }
        private async void therapist(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TherapistPage());
        }
        private async void editProfile(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditUserProfile());
        }
    }
}