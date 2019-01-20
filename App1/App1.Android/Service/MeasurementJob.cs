﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.JobDispatcher;
using Plugin.LocalNotifications;
using Xamarin.Android;

namespace App1.Droid.Service
{
    [Service(Name = "firebaseJobDispatcher.MeasurementJob")]
    [IntentFilter(new[] { FirebaseJobServiceIntent.Action })]
    class MeasurementJob : JobService
    {
        public static readonly string TAG = typeof(MeasurementJob).FullName; //unique TAG

        public override bool OnStartJob(IJobParameters jobParameters)
        {
            Task.Run(async () =>
            {
                TestMeViewModel testMeViewModel = new TestMeViewModel();
                await MeasurementHandler.ResendIntervals(); //resend previous measurements if exist
                await MeasurementHandler.GetStressResult(1, testMeViewModel); //start new measurement. -1 => real measurement
                String[] stressRes = testMeViewModel.StressResult.Split(" ");
                int tempLen = stressRes.Length;
                if (stressRes[tempLen - 2] != "not") {
                    CrossLocalNotifications.Current.Show("RelaxApp noticed stress", "Tap into the app for more information");
                }
            });
            return true;
        }

        public override bool OnStopJob(IJobParameters jobParameters)
        {
            return false;
        }

    }
}