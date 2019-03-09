//using Microsoft.Band;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace App1
{
    public interface IBand
    {
        Task<bool> ConnectToBand(TestMeViewModel b);
        Task<int> getHR(TestMeViewModel b);
        Task<int> getGSR(TestMeViewModel b, int sec);
        Task<bool> readRRSensor(TestMeViewModel b, int sec);
        List<int> GsrReadings();
        List<double> RRIntervalReadings();
        void ClearAllReadings();
        Task RequestConsent();
        void SendVibration();
    }
    public interface IBluetooth
    {
        bool TurnOn();
        bool TurnOff();
    }
    public interface ILocation
    {
        Task<Android.Locations.Location> GetLastLocationFromDevice();
    }
    public interface ISchedule
    {
        void ScheduleMeasurement(int minutes); //Schedule measurement every x minutes
    }

    public interface IContacts
    {
        Task<List<ContactLists>> GetDeviceContactsAsync();
    }
}