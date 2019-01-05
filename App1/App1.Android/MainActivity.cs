﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Microsoft.Band;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android;
using Android.Support.Design.Widget;
using Xamarin.Forms.Maps;

namespace App1.Droid
{
    [Activity(Label = "App1", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate

    {
        public static Android.Content.Context context;
        public static Activity instance;


        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Google login using a server-managed flow.
                var user = await Login.Default.ServiceClient.LoginAsync(this,
                    MobileServiceAuthenticationProvider.Google, "androidrelaxapp");
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
        

        protected override void OnCreate(Bundle savedInstanceState)
        {


            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            context = this.ApplicationContext;
            instance = this;
            LoadApplication(new App());

            global::Xamarin.FormsMaps.Init(this, savedInstanceState);


            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            App.Init((IAuthenticate)this); //create IAuthenticate object in App.cs


        }

    }
}