using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace App1.Services
{
    class AzureDataService
    {
        private MobileServiceClient _mobileServiceClient = new MobileServiceClient("https://relaxapp.azurewebsites.net/");
        IMobileServiceSyncTable<Activities> _activities;
        IMobileServiceSyncTable<Measurements> _measurements;

        public async Task Initialize()
        {
            if (_mobileServiceClient?.SyncContext?.IsInitialized ?? false)
                return;

            const string path = "syncstore.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);
            store.DefineTable<Activities>();
            store.DefineTable<Measurements>();
            await _mobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            _activities = _mobileServiceClient.GetSyncTable<Activities>();
            _measurements = _mobileServiceClient.GetSyncTable<Measurements>();
        }

        public async Task<IEnumerable<Activities>> GetActivities()
        {
            await Initialize();
            await SyncActivties();
            return await _activities.ToEnumerableAsync();
        }

        public async Task<IEnumerable<Measurements>> GetMeasurements()
        {
            await Initialize();
            await SyncMeasurements();
            return await _measurements.ToEnumerableAsync();
        }


        public async Task AddActivity(Activities activity)
        {
            await Initialize();

            await _activities.InsertAsync(activity);

            await SyncActivties();
        }

        public async Task AddMeasurement(Measurements m)
        {
            await Initialize();

            await _measurements.InsertAsync(m);

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
    }


}
