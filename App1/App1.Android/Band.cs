using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.App;
using Android.Widget;

/*
 * this class implements the BandInterface interface.
 * it is used to get data from the band, using the Microsoft.Band SDK
 * we save multiple measurements in a list since we might need mean and SD
 * when reading data from each sensor (HR or GSR), we also check if the BandContactState is changed
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
        private bool _gsrDone = false;
        private BandContactState _bandState = null;
        public List<int> windowGsr = new List<int>();

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
                    Toast.MakeText(Droid.MainActivity.context,
                        "make sure your band is paired with this device and try again",
                        ToastLength.Short).Show();
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
            InitSensors(b); //register sensors listeners
            return _client.IsConnected;
        }

        //assumes the band is connected
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
        //assumes the band is connected
        public async Task<int> getGSR(BaseViewModel b, int sec)
        {
            Activity activity = Droid.MainActivity.instance;
            _contactSensor.StartReadings();
            while (_bandState == null) { }
            if (_bandState != BandContactState.Worn)
            {
                _bandState = null;
                return -1;
            }
            _gsrDone = false;
            _gsrReadings.Clear();
            b.GsrList = "#";

            _gsrSensor.StartReadings();
            await Task.Delay(sec * 1000);
            _gsrDone = true;
            _contactSensor.StopReadings();
            return 0;
        }

        public List<int> getList()
        {
            return _gsrReadings;
        }
        public void Clear()
        {
            _gsrReadings.Clear();
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
                    b.GsrList = gsrEvent.Resistance.ToString();

                    if (_gsrDone)
                    {
                        _gsrSensor.StopReadings();
                        _contactSensor.StopReadings();
                        //b.GSR = gsrEvent.Resistance;
                        return;
                    }
                });
            };
        }
    }
}
