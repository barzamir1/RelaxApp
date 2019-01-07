using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            //_mobileServiceClient = new MobileServiceClient("https://relaxapp.azurewebsites.net");
            _mobileServiceClient = Services.AzureDataService.Instance._mobileServiceClient;
            //const string path = "syncstore.db";

            ////setup our local sqlite store and initialize our table
            //var store = new MobileServiceSQLiteStore(path);
            //store.DefineTable<Users>();
            //ServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            UsersTable = ServiceClient.GetSyncTable<Users>();
            UsersTable.PullAsync("Users", UsersTable.CreateQuery());

            RetrieveUserFromLocalStorage();
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
        async void loginButton_Clicked(object sender, EventArgs e)
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
                    
                    //save user id to local storage
                    Application.Current.Properties["user_id "] = userId; 
                    Application.Current.Properties["user_token "] = _mobileServiceClient.CurrentUser.MobileServiceAuthenticationToken;
                    await Application.Current.SavePropertiesAsync();

                    if (currentUser != null)
                        await Navigation.PushAsync(new Page1()); //navigate to home page
                    else
                    {
                        currentUser = new Users();
                        currentUser.id = userId;
                        await Navigation.PushAsync(new Signup()); //navigate to registration page
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }   
        }
       async void RetrieveUserFromLocalStorage()
        {
            try
            {
                //check if the user previously signed in
                var currentuserID = (Application.Current.Properties["user_id "]); //get user id from local storage
                var token = (Application.Current.Properties["user_token "]);
                if (currentuserID != null)
                {
                    _mobileServiceClient.CurrentUser = new MobileServiceUser(currentuserID.ToString());
                    _mobileServiceClient.CurrentUser.MobileServiceAuthenticationToken = token.ToString();
                    CurrentUser = await UsersTable.LookupAsync(currentuserID.ToString());
                    await Navigation.PushAsync(new Page1()); //navigate to home page
                }
            }
            catch (Exception e)//no user id in local storage. user must press the Login button
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}