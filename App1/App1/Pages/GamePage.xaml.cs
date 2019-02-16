using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        bool measurementStarted = false;
        public GamePage()
        {
            InitializeComponent();
            webView.Source = "https://www.cubeslam.com/"; //"https://monsterofcookie.itch.io/thug-racer";
            webView.Navigated += WebView_Navigated;
            
        }

        private void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            try
            {
                //Action onFinish = new Action(() => { Navigation.PushAsync(new Page1()); Navigation.RemovePage(this); });
                Action onFinish = new Action(() => { buttonContinue.IsVisible = true; });
                var thread = new Thread(async() => {
                    await Task.Delay(30 * 1000); //wait 30 seconds before start measuring
                    TestMeViewModel b = new TestMeViewModel();
                    b.Progress = 0;
                    MeasurementHandler.GetStressResult(-1, b);
                    while (b.IsFinished == false) {
                        if (b.StressResult.StartsWith("Error"))
                        {
                            //TODO: alert user and try again
                            return;
                        }
                    }
                    //int repetitionTime = MeasurementHandler.measureRepetitionTime;
                    //DependencyService.Get<ISchedule>().ScheduleMeasurement(repetitionTime); //Schedule measurement every 6 minutes
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(onFinish);

                });
                thread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void ButtonContinue_Clicked(object sender, EventArgs e)
        {
            //measurement is over. continue to main page
            await Navigation.PushAsync(new Page1());
            Navigation.RemovePage(this); //no going back
        }
    }
}