using System;
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
                await MeasurementHandler.ResendIntervals(); //resend previous measurements if exist
                MeasurementHandler.GetStressResult(-1, null); //start new measurement. -1 => real measurement
            });
            return true;
        }

        public override bool OnStopJob(IJobParameters jobParameters)
        {
            return false;
        }
    }
}