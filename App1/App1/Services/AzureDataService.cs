using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
        IMobileServiceSyncTable<Users> _usersTable;


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
            const string path = "syncstore.db";
            //setup our local sqlite store and initialize our table
            MobileServiceSQLiteStore store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Activities>();
            store.DefineTable<Measurements>();
            store.DefineTable<Users>();
            await _mobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            _activities = _mobileServiceClient.GetSyncTable<Activities>();
            _measurements = _mobileServiceClient.GetSyncTable<Measurements>();
            _usersTable = _mobileServiceClient.GetSyncTable<Users>();
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

        public async Task<IEnumerable<Measurements>> GetMeasurements()
        {
            if (currentUserID == null)
                currentUserID = Login.Default.CurrentUser.id;
            await SyncMeasurements();
            return await _measurements.Where(item=>item.UserID==currentUserID).ToEnumerableAsync();
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
                await _measurements.PullAsync("Measurements", _measurements.Where(item => item.UserID == currentUserID));
                await _mobileServiceClient.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}

