﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

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

        //private void ButtonConnect_Clicked(object sender, EventArgs e)
        //{
        //    TestMeViewModel b = (TestMeViewModel)BindingContext;

        //    Task<bool> isConnected = DependencyService.Get<BandInterface>().ConnectToBand(b);
        //    //this.buttonConnect.IsEnabled = !isConnected.Result;
        //}

        //private void ButtonHR_Clicked(object sender, EventArgs e)
        //{
        //    TestMeViewModel b = (TestMeViewModel)BindingContext;
        //    b.HR = 0;
        //    Task<int> o = DependencyService.Get<BandInterface>().getHR(b);
        //}

        //private void ButtonGSR_Clicked(object sender, EventArgs e)
        //{
        //    TestMeViewModel b = (TestMeViewModel)BindingContext;
        //    b.GSR = 0;
        //    Task<int> o = DependencyService.Get<BandInterface>().getGSR(b, 3);
        //}

        //private async void ButtonGSRWindow_Clicked(object sender, EventArgs e)
        //{
        //    TestMeViewModel b = (TestMeViewModel)BindingContext;
        //    b.GSR = 0;
        //    this.buttonGSRWindow.IsEnabled = false;
        //    await DependencyService.Get<BandInterface>().getGSR(b,40);
        //    //List<int> windowReads = DependencyService.Get<BandInterface>().getList();
        //    this.buttonGSRWindow.IsEnabled = true;
        //}

        //private void ButtonRRInterval_Clicked(object sender, EventArgs e)
        //{
        //    ////this.buttonGSRWindow.IsEnabled = false;
        //    //TestMeViewModel b = (TestMeViewModel)BindingContext;
        //    //b.PNN50 = 0;
        //    //double PNN50 = 0;
        //    //Action onCompleted = () =>
        //    //{
        //    //    b.PNN50 = PNN50;
        //    //};
        //    ////don't make the UI thread wait
        //    //var thread = new Thread(
        //    //  () =>
        //    //  {
        //    //      try
        //    //      {
        //    //         PNN50 = StressCalculator.CalcStressIndex();
        //    //      }
        //    //      finally
        //    //      {
        //    //          onCompleted();
        //    //      }
        //    //  });
        //    //thread.Start();
        //}

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
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                if (b.Progress < 1)
                {
                    msPass += 50;
                    b.Progress = msPass / (90 * 1000);
                    //b.Progress += 50; //update progress every 50ms
                    return true;
                }
                if (isSignup)
                {
                    if (b.StressResult.StartsWith("you")) //succeeded
                    { 
                        Navigation.PushAsync(new Pages.SignupStressTest());
                        Navigation.RemovePage(this); //user can't go back
                    }
                }
                return false;
            });
        }

        private void ButtonRRrelax_Clicked(object sender, EventArgs e)
        {
            stressResult = "";
            StartMeasure(0);
           // progressBar.Progress = 0;
            //labelStressResults.Text = "Reading...";
           // await progressBar.ProgressTo(1.0, 1000 * 90, Easing.Linear); //90 seconds
           // labelStressResults.Text = stressResult;
        }

        private void ButtonRRstress_Clicked(object sender, EventArgs e)
        {
            stressResult = "";
            StartMeasure(1);
            //progressBar.Progress = 0;
            //labelStressResults.Text = "Reading...";
            //await progressBar.ProgressTo(1.0, 1000 * 90, Easing.Linear); //90 seconds
            //labelStressResults.Text = stressResult;
        }
        private void ButtonReal_Clicked(object sender, EventArgs e)
        {
            stressResult = "";
            StartMeasure(-1); //real measurement
           
            //progressBar.Progress = 0;
            //labelStressResults.Text = "Reading...";
            //await progressBar.ProgressTo(1.0, 1000 * 90, Easing.Linear); //90 seconds
            //labelStressResults.Text = stressResult;
        }
    }
}
