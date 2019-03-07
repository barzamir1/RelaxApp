using Android.App;
using Android.Content.PM;
using Android.Gms.Location;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace App1.Droid
{
    [Activity(Label = "RelaxApp", Icon = "@drawable/relaxAppLogo", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        public static Android.Content.Context context;
        public static Activity instance;

        public static FusedLocationProviderClient fusedLocationProviderClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            //To get the current location
            fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            context = this.ApplicationContext;
            instance = this;

            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            App.Init((IAuthenticate)this); //create IAuthenticate object in App.cs
            
            //For Maps
            global::Xamarin.FormsMaps.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Google login using a server-managed flow.
                var user = await Login.Default.ServiceClient.LoginAsync(this, MobileServiceAuthenticationProvider.Google,
                    "androidrelaxapp", new Dictionary<string, string>() { { "access_type", "offline" } });

                if (user != null)
                    success = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            if (message.Length > 0)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(message);
                builder.SetTitle("Sign-in result");
                builder.Create().Show();
            }
            return success;
        }

        public override void OnBackPressed()
        {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
            {
                // Do something if there are some pages in the `PopupStack`
            }
            else
            {
                // Do something if there are not any pages in the `PopupStack`
            }
        }

        //public void ScheduleMeasurement(int minutes)
        //{
        //    dispatcher = this.CreateJobDispatcher();
        //    JobTrigger.ExecutionWindowTrigger trigger = Firebase.JobDispatcher.Trigger.ExecutionWindow(minutes * 60, minutes * 60 + 10);

        //    var job = dispatcher.NewJobBuilder()
        //                        .SetService<MeasurementJob>("measurement-service")
        //                        .SetRecurring(true)
        //                        .SetTrigger(trigger)
        //                        .SetLifetime(Lifetime.Forever)
        //                        .Build();

        //    int result = dispatcher.Schedule(job);
        //    if (result == FirebaseJobDispatcher.ScheduleResultSuccess)
        //        Console.WriteLine("Job succeeded");
        //    else
        //        Console.WriteLine("Job failed");
        //}
    }
}