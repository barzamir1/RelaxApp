using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.App;

/*
 * this class implements the BandInterface interface.
 * it is used to get data from the band, using the Microsoft.Band SDK
 * we save multiple measurments in a list since we might need mean and sd
 * when reading data from each sensor (HR or GSR), we also cheack if the BandContactState is changed
 * by reading the contact sensor, so we could terminate in case the band is no longer worn.
 */

[assembly: Dependency(typeof(App1.Band_Android))]
namespace App1
{

    public class Band_Android : BandInterface
    {
        private static IBandClient _client = null; //our band
        private List<int> _hrReadings = new List<int>(); 
        private List<int> _gsrReadings = new List<int>();
        private long _gsrTimeStamp = 0; //the first gsr measurment
        private BandContactState _bandState = null;
        
        //sensors
        private HeartRateSensor _hrSensor;
        private GsrSensor _gsrSensor;
        private ContactSensor _contactSensor;

        public async Task<bool> ConnectToBand(BaseViewModel b)
        {
            if (_client == null)
            {
                //get all paired devices (we only care about the 1st)
                IBandInfo[] devices = BandClientManager.Instance.GetPairedBands();
                if (devices.Length == 0)
                {
                    b.IsConnected = false;
                    return false;
                }
                _client = BandClientManager.Instance.Create(Droid.MainActivity.context, devices[0]);
            }
            else if (_client.ConnectionState == ConnectionState.Connected)
            {
                return true;
            }
            //connecting to device
            ConnectionState connectionState = await _client.ConnectTaskAsync();
            b.IsConnected = _client.IsConnected; //update BaseViewModel
            InitSensors(b); //register sensors liteners
            return _client.IsConnected;
        }

        public async Task<int> getHR(BaseViewModel b)
        {
            Activity activity = Droid.MainActivity.instance;
            _contactSensor.StartReadings();
            while (_bandState == null) { } 
            if (_bandState != BandContactState.Worn)
            {
                _bandState = null;
                return -1;
            }
            _hrSensor.StartReadings();
            return 0;
        }
        public async Task<int> getGSR(BaseViewModel b)
        {
            Activity activity = Droid.MainActivity.instance;
            _contactSensor.StartReadings();
            while (_bandState == null) { }
            if (_bandState != BandContactState.Worn)
            {
                _bandState = null;
                return -1;
            }
            _gsrSensor.StartReadings();
            return 0;
        }

        public async void InitSensors(BaseViewModel b)
        {
            if (!_client.IsConnected)
                return;
            _hrSensor = _client.SensorManager.CreateHeartRateSensor();
            _gsrSensor = _client.SensorManager.CreateGsrSensor();
            _contactSensor = _client.SensorManager.CreateContactSensor();

            if (_contactSensor == null || _hrSensor == null || _gsrSensor == null)
                return;
            Activity activity = Droid.MainActivity.instance;

            //check user's consent to read HR. should only occur once
            if (_client.SensorManager.CurrentHeartRateConsent != UserConsent.Granted)
            {
                if (!await _client.SensorManager.RequestHeartRateConsentTaskAsync(activity))
                {
                    Console.WriteLine("ERROR: Can't get user's consent to read HR");
                    return;
                }
            }

            //register contact listener
            _contactSensor.ReadingChanged += (sender, e) =>
            {
                var contactEvent = e.SensorReading;
                _bandState = contactEvent.ContactState;
            };
            //register heart rate listener
            _hrSensor.ReadingChanged += (sender, e) =>
            {
                activity.RunOnUiThread(() =>
                {
                    var heartRateEvent = e.SensorReading;
                    _hrReadings.Add(heartRateEvent.HeartRate);
                    if (heartRateEvent.Quality == HeartRateQuality.Locked)
                    {
                        _hrSensor.StopReadings();
                        _contactSensor.StopReadings();
                        b.HR = _hrReadings[_hrReadings.Count - 1]; //update ViewModel
                    }
                    if (_bandState != BandContactState.Worn) //user took off the band while reading
                    {
                        _hrSensor.StopReadings();
                        _contactSensor.StopReadings();
                        _bandState = null;
                        return;
                    }
                });
            };
            //register gsr listener
            _gsrSensor.ReadingChanged += (sender, e) =>
             {
                 activity.RunOnUiThread(() =>
                  {
                      var gsrEvent = e.SensorReading;
                      _gsrReadings.Add(gsrEvent.Resistance);
                      if (_gsrTimeStamp == 0)
                          _gsrTimeStamp = gsrEvent.Timestamp; //the first measurement
                     if (gsrEvent.Timestamp - _gsrTimeStamp > 6 * 1000) //read for 6 sec.
                     {
                          _gsrSensor.StopReadings();
                          _contactSensor.StopReadings();
                          b.GSR = _gsrReadings[_gsrReadings.Count - 1]; //update ViewModel
                     }
                      if (_bandState != BandContactState.Worn) //user took off the band while reading
                     {
                          _gsrSensor.StopReadings();
                          _contactSensor.StopReadings();
                          _bandState = null;
                          return;
                      }
                  });
             };
        }

    }
}
