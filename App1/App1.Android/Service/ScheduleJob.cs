using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.JobDispatcher;
using Xamarin.Forms;

[assembly: Dependency(typeof(App1.Droid.Service.ScheduleJob))]
namespace App1.Droid.Service
{
    class ScheduleJob: ISchedule
    {
        FirebaseJobDispatcher dispatcher;

        public void ScheduleMeasurement(int minutes)
        {
            dispatcher = MainActivity.instance.CreateJobDispatcher();
            JobTrigger.ExecutionWindowTrigger trigger = Firebase.JobDispatcher.Trigger.ExecutionWindow(minutes * 60, minutes * 60 + 10);

            var job = dispatcher.NewJobBuilder()
                                .SetTag("MeasurementService") //unique tag
                                .SetService<MeasurementJob>("measurement-service")
                                .SetRecurring(true)
                                .SetTrigger(trigger)
                                .SetLifetime(Lifetime.Forever)
                                .SetReplaceCurrent(true) //replace previous defined job
                                .Build();

            int result = dispatcher.Schedule(job);
            if (result == FirebaseJobDispatcher.ScheduleResultSuccess)
                Console.WriteLine("Job succeeded");
            else
                Console.WriteLine("Job failed");
        }
    }
}