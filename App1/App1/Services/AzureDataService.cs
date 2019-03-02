using Android.Util;
using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1.Services
{
    class AzureDataService
    {
        static AzureDataService instance;
        //public MobileServiceClient _mobileServiceClient = Login.Default.ServiceClient;
        String currentUserID;
        public MobileServiceClient _mobileServiceClient = new MobileServiceClient("https://relaxapp.azurewebsites.net/");
        public IMobileServiceSyncTable<Activities> _activities;
        IMobileServiceSyncTable<Measurements> _measurements;
        IMobileServiceSyncTable<Users> _users;
        IMobileServiceSyncTable<UserAuthorizations> _userAuthorizations;

        public static AzureDataService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureDataService();
                    instance.Initialize();
                }
                return instance;
            }
        }
        public async Task Initialize()
        {
            //string path = "syncstore" + DateTime.Now.Second + ".db";
            string path = "syncstore.db";
            //setup our local sqlite store and initialize our table
            MobileServiceSQLiteStore store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Activities>();
            store.DefineTable<Measurements>();
            store.DefineTable<Users>();
            store.DefineTable<UserAuthorizations>();
            await _mobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            _activities = _mobileServiceClient.GetSyncTable<Activities>();
            _measurements = _mobileServiceClient.GetSyncTable<Measurements>();
            _users = _mobileServiceClient.GetSyncTable<Users>();
            _userAuthorizations = _mobileServiceClient.GetSyncTable<UserAuthorizations>();
            if (_mobileServiceClient.CurrentUser != null)
                currentUserID = _mobileServiceClient.CurrentUser.UserId;
        }

        public async Task<IEnumerable<Activities>> GetActivities()
        {
            //await Initialize();
            await SyncActivties();
            return await _activities.ToEnumerableAsync();
        }
        public async Task<List<Activities>> GetActivitiesList()
        {
            //await Initialize();
            await SyncActivties();
            return await _activities.ToListAsync();
        }

        public async Task<List<Users>> GetAllUsers()
        {
            //await Initialize();
            await SyncUsers();
            return await _users.ToListAsync();
        }

        public async Task SyncUsers()
        {
            try
            {
                await _users.PullAsync("Users", _users.CreateQuery());
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<List<UserAuthorizations>> GetAllAuthUsers()
        {
            //await Initialize();
            currentUserID = Login.Default.CurrentUser.id;
            await SyncAuthUsers();
            return await _userAuthorizations.ToListAsync();
        }

        public async Task SyncAuthUsers()
        {
            try
            {
                await _userAuthorizations.PullAsync("UserAuthorizations", 
                    _userAuthorizations.Where(item => item.TherapistID==currentUserID));
                
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<IEnumerable<Measurements>> GetMeasurements()
        {
            var currUser = Login.Default.CurrentUser;
            if (currUser.isTherapist)
                currentUserID = currUser.WatchingUserID;
            else
                currentUserID = currUser.id;
            await SyncMeasurements();
            return await _measurements.Where(item => item.UserID == currentUserID)
                .OrderByDescending(item=>item.Date).ToEnumerableAsync();
        }
        public async Task<List<Measurements>> GetMeasurementsList()
        {
            _measurements = _mobileServiceClient.GetSyncTable<Measurements>();
            currentUserID = Login.Default.CurrentUser.id;
            await SyncMeasurements();
            return await _measurements.ToListAsync();
        }

        public async Task AddActivity(Activities activity)
        {
            //await Initialize
            await _activities.InsertAsync(activity);
            //await SyncActivties();
        }

        public async Task AddMeasurement(Measurements m)
        {
            //await Initialize
            await _measurements.InsertAsync(m);
            await SyncMeasurements();
        }
        public async Task AddAuthUser(UserAuthorizations userAuthorizations)
        {
            try
            {
                await _userAuthorizations.InsertAsync(userAuthorizations);
                await SyncAuthUsers();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task UpdateMeasurement(Measurements m)
        {
            await _measurements.UpdateAsync(m);
            await SyncMeasurements();
        }

        public async Task SyncActivties()
        {
            try
            {
                await _activities.PullAsync("Activities", _activities.CreateQuery());
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task SyncMeasurements()
        {
            try
            {
                await _measurements.PullAsync("Measurements", _measurements.CreateQuery());
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public async Task<bool> UpdateUser(Users user)
        {
            try
            {
                await _users.UpdateAsync(user);
                await SyncUsers();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

