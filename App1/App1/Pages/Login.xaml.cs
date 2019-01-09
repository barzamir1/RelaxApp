﻿using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        bool authenticated;
        static Login defaultInstance;
        MobileServiceClient _mobileServiceClient;
        IMobileServiceSyncTable<Users> UsersTable;
        Users currentUser = null;

        public Login()
        {
            InitializeComponent();
            Default = this;
            _mobileServiceClient = Services.AzureDataService.Instance._mobileServiceClient;

            //Get our sync table that will call out to azure
            UsersTable = ServiceClient.GetSyncTable<Users>();
            UsersTable.PullAsync("Users", UsersTable.CreateQuery());

            //try to load current user from mobileServiceClient
            RetrieveUser();
        }

        //class data members
        public static Login Default
        {
            get
            {
                defaultInstance = defaultInstance ?? new Login();
                return defaultInstance;
            }
            private set { defaultInstance = value; }
        }
        public MobileServiceClient ServiceClient
        {
            get { return _mobileServiceClient; }
        }
        public Users CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; }
        }

        //functions
        async void RetrieveUser()
        {
            if (_mobileServiceClient.CurrentUser != null)
            {
                String userId = ServiceClient.CurrentUser.UserId.Substring(4);
                await UsersTable.PullAsync("Users", UsersTable.CreateQuery());
                currentUser = await UsersTable.LookupAsync(userId);
                try { await ServiceClient.RefreshUserAsync(); }
                catch { return; /*refresh doesn't have to work*/}
                NavigateNextPage();
            }
        }
        async void LoginButton_Clicked(object sender, EventArgs e)
        {
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.Authenticate();
            try
            {
                if (authenticated)
                {
                    String userId = ServiceClient.CurrentUser.UserId.Substring(4);
                    await UsersTable.PullAsync("Users", UsersTable.CreateQuery());
                    currentUser = await UsersTable.LookupAsync(userId); //check if user exist in Users table
                    NavigateNextPage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        async void NavigateNextPage()
        {
            if (CurrentUser == null) //user doesn't exist in DB
            {
                currentUser = new Users();
                currentUser.id = ServiceClient.CurrentUser.UserId.Substring(4);
                await Navigation.PushAsync(new Signup()); //navigate to registration page
            }
            else
                //await Navigation.PushAsync(new Pages.SignupRelaxTest());
                await Navigation.PushAsync(new Page1()); //navigate to home page
            Navigation.RemovePage(this); //no going back
        }
    }
}