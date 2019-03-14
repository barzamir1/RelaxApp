using Microsoft.Band;
using Microsoft.Band.Sensors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Android.App;
using Android.Widget;
using Android.Util;

/*
 * this class implements the BandInterface interface.
 * it is used to get data from the band, using the Microsoft.Band SDK
 * we save multiple measurements in a list since we need mean and SD
 * when reading data from each sensor (HR or GSR), we also check if the BandContactState is changed
 * by reading the contact sensor, so we could terminate in case the band is no longer worn.
 */

[assembly: Dependency(typeof(App1.Band_Android))]
namespace App1
{

    public class Band_Android : IBand
    {
        private static IBandClient _client = null; //our band
        private List<int> _hrReadings = new List<int>();
        private List<int> _gsrReadings = new List<int>();
        private List<double> _rrIntervalsReadings = new List<double>();
        private bool _gsrDone = false;
        private MotionType currentMotionTyp;
        private BandContactState _bandState = null;
        public List<int> windowGsr = new List<int>();


        //sensors
        private HeartRateSensor _hrSensor;
        private GsrSensor _gsrSensor;
        private RRIntervalSensor _rrSensor;
        private ContactSensor _contactSensor;
        private DistanceSensor _distancerSensor;


        public async Task<bool> ConnectToBand(TestMeViewModel b)
        {
            try
            {
                if (_client == null)
                {
                    //get all paired devices (we only care about the 1st)
                    IBandInfo[] devices = BandClientManager.Instance.GetPairedBands();
                    if (devices.Length == 0)
                    {
                        if (b != null) { b.IsConnected = false; }
                        Activity activity = Droid.MainActivity.instance;
                        activity.RunOnUiThread(() =>
                        {
                            Toast.MakeText(Droid.MainActivity.context,
                            "make sure your band is paired with this device and try again",
                            ToastLength.Long).Show();
                        });
                        return false;
                    }
                    _client = BandClientManager.Instance.Create(Droid.MainActivity.context, devices[0]);
                }
                else if (_client.ConnectionState == ConnectionState.Connected)
                {
                    if (b != null) { b.IsConnected = true; }
                    return true;
                }
                //connecting to device
                ConnectionState connectionState = await _client.ConnectTaskAsync();
                if (b != null) { b.IsConnected = _client.IsConnected; } //update TestMeViewModel
                InitSensors(b); //register sensors listeners
                return _client.IsConnected;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<int> getHR(TestMeViewModel b)
        {
            await ConnectToBand(b);
            InitSensors(b);
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
        public async Task<int> getGSR(TestMeViewModel b, int sec)
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

        public async Task<bool> readRRSensor(TestMeViewModel b, int sec)
        {
            Activity activity = Droid.MainActivity.instance;
            _contactSensor.StartReadings();
            while (_bandState == null) { } //get the band state
            if (_bandState != BandContactState.Worn)
            {
                _bandState = null;
                //TODO: notify the user by sending a notification
                return false;
            }
            _distancerSensor.StartReadings();
            while(currentMotionTyp == null) { }
            if (currentMotionTyp == MotionType.Walking || currentMotionTyp == MotionType.Jogging || currentMotionTyp == MotionType.Running)
            {
                b.StressResult = "Error: can't measure during sports activity";
                Log.Error("motion", "user is " + currentMotionTyp.ToString());
                return false;
            }
            _rrIntervalsReadings.Clear();
            //RequestConsent();
            _rrSensor.StartReadings();
            await Task.Delay(sec * 1000);
            _rrSensor.StopReadings();
            _contactSensor.StopReadings();
            return true;
        }

        public List<int> GsrReadings()
        {
            return _gsrReadings;
        }
        public List<double> RRIntervalReadings()
        {
            return _rrIntervalsReadings;
        }
        public void ClearAllReadings()
        {
            _gsrReadings.Clear();
            _rrIntervalsReadings.Clear();
            _hrReadings.Clear();
        }
        public void InitSensors(TestMeViewModel b)
        {
            if (!_client.IsConnected)
                return;
            _hrSensor = _hrSensor ?? _client.SensorManager.CreateHeartRateSensor();
            _gsrSensor = _gsrSensor ?? _client.SensorManager.CreateGsrSensor();
            _rrSensor = _rrSensor ?? _client.SensorManager.CreateRRIntervalSensor();
            _contactSensor = _contactSensor ?? _client.SensorManager.CreateContactSensor();
            _distancerSensor = _distancerSensor ?? _client.SensorManager.CreateDistanceSensor();

            if (_contactSensor == null || _hrSensor == null || _gsrSensor == null || _rrSensor == null || _distancerSensor == null)
                return;
            Activity activity = Droid.MainActivity.instance;

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
                    //_hrReadings.Add(heartRateEvent.HeartRate);
                    if (heartRateEvent.Quality == HeartRateQuality.Locked)
                    {
                        _hrSensor.StopReadings();
                        _contactSensor.StopReadings();
                        // if (b != null) { b.HR = _hrReadings[_hrReadings.Count - 1]; } //update ViewModel
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
                    if (b != null) { b.GsrList = gsrEvent.Resistance.ToString(); }

                    if (_gsrDone)
                    {
                        _gsrSensor.StopReadings();
                        _contactSensor.StopReadings();
                        return;
                    }
                });
            };
            //register RR Intervals listener
            _rrSensor.ReadingChanged += (sender, e) =>
            {
                if (_bandState != BandContactState.Worn) //user took off the band while reading
                {
                    _rrSensor.StopReadings();
                    _contactSensor.StopReadings();
                    _bandState = null;
                    b.StressResult = "Error: band is not worn.";
                    return;
                }
                var rrEvent = e.SensorReading;
                _rrIntervalsReadings.Add(rrEvent.Interval);
            };
            _distancerSensor.ReadingChanged += (sender, e) =>
            {
                currentMotionTyp = e.SensorReading.MotionType;
                _distancerSensor.StopReadings();
            };
        }
        public async Task RequestConsent()
        {
            //check user's consent to read HR. should only occur once
            if (_client.SensorManager.CurrentHeartRateConsent != UserConsent.Granted)
            {
                Activity activity = Droid.MainActivity.instance;
                if (!await _client.SensorManager.RequestHeartRateConsentTaskAsync(activity))
                {
                    Console.WriteLine("ERROR: Can't get user's consent to read HR");
                    return;
                }
            }
        }

        public void SendVibration()
        {
            if(_client!=null)
                _client.NotificationManager.VibrateAsync(Microsoft.Band.Notifications.VibrationType.NotificationOneTone);
        }
    }
}
