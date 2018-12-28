using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _mobileServiceClient = new MobileServiceClient("https://relaxapp.azurewebsites.net");
            const string path = "syncstore.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Users>();

            ServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            UsersTable = ServiceClient.GetSyncTable<Users>();
            UsersTable.PullAsync("Users", UsersTable.CreateQuery());
        }

        public static Login Default
        {
            get { return defaultInstance; }
            private set { defaultInstance = value; }
        }
        public MobileServiceClient ServiceClient
        {
            get { return _mobileServiceClient; }
        }
        public Users CurrentUser
        {
            get { return currentUser; }
        }

        async void loginButton_Clicked(object sender, EventArgs e)
        {
            if (App.Authenticator != null)
                authenticated = await App.Authenticator.Authenticate();
            try
            {
                if (authenticated)
                {
                    String userId = ServiceClient.CurrentUser.UserId.Substring(4);
                    currentUser = await UsersTable.LookupAsync(userId);
                    await Navigation.PushAsync(new Page1()); //navigate to home page
                }
                else
                {
                    //navigate to registration page
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }   
        }
    }
}