using System;
using System.Threading;
using Xamarin.Forms;

/*
 * this class is only used for Debugging.
 * it displays the HR and GSR measurements in the UI.
 * -TestMeViewModel updates the controls in TestMe.xaml when one of its properties changes.
 * -DependencyService.Get<BandInterface>().func() is the way we use an android-only class in Xamarin
 * we created an Interface BandInterface and implemented the functions in Band.cs (android-class) 
 */
namespace App1
{
    public partial class TestMe : ContentPage
    {
        String stressResult = "";
        bool isSignup;
        public TestMe(bool isSignup)
        {
            InitializeComponent();
            BindingContext = new TestMeViewModel();
            this.isSignup = isSignup;
            if (isSignup)
                StartMeasure(-1); //auto start
        }

        private void StartMeasure(int pseudo)
        {
            TestMeViewModel b = (TestMeViewModel)BindingContext;
            b.PNN50 = 0;
            //don't make the UI thread wait
            var thread = new Thread(
              () =>
              {
                  MeasurementHandler.GetStressResult(pseudo, b);
              });

            b.Progress = 0;
            thread.Start();
            double msPass = 0;
            int testTime = MeasurementHandler.testTimeInSec;
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (b.Progress < 1)
                {
                    msPass += 50;
                    b.Progress = msPass <= testTime * 1000 ? msPass / (testTime * 1000) : (msPass - 1) / msPass;
                    if (b.StressResult.StartsWith("Error")) { return false; }
                    return true;
                }
                if (isSignup)
                {
                    if (b.StressResult.StartsWith("you")) //succeeded
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PushAsync(new Pages.SignupStressTest());
                            Navigation.RemovePage(this); //user can't go back});
                        });
                        return false;
                    }
                    return true;
                }
                else
                    return false;
                
            });
        }
    

        private void ButtonRRrelax_Clicked(object sender, EventArgs e)
        {
            stressResult = "";
            StartMeasure(0);
        }

        private void ButtonRRstress_Clicked(object sender, EventArgs e)
        {
            stressResult = "";
            StartMeasure(1);
        }
        private void ButtonReal_Clicked(object sender, EventArgs e)
        {
            stressResult = "";
            StartMeasure(-1); //real measurement
        }       
    }
}
