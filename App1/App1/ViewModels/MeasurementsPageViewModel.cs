using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using App1.DataObjects;
using App1.Services;
using Xamarin.Forms;

namespace App1.ViewModels
{
    class MeasurementsPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Measurements> _measurements;
        private string _userID;
        private DateTime _date;
        private int _tRI;
        private double _pNN50;
        private double _sDNN;
        private double _sDSD;
        private string _activityID;
        private int _stressIndex;
        private int _isStressed;
        private double _gPSLat;
        private double  _gPSLng; 
        private ICommand _addMeasurement;
        //private AzureDataService _azureDataService = new AzureDataService();
        private AzureDataService _azureDataService = AzureDataService.Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public MeasurementsPageViewModel()
        {
            MeasurementsObj = new ObservableCollection<Measurements>();

            InitializeMeasurement();
        }

        private async void InitializeMeasurement()
        {
            var measurement = await _azureDataService.GetMeasurements();

            foreach (var measure in measurement)
            {
                MeasurementsObj.Add(measure);
            }
        }

        public ObservableCollection<Measurements> MeasurementsObj
        {
            get { return _measurements; }
            set
            {
                _measurements = value;
                OnPropertyChanged("Measurements");
            }
        }

        public string UserID
        {
            get { return UserID; }
            set
            {
                _userID = value;
                OnPropertyChanged("UserID");
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        public int TRI
        {
            get { return _tRI; }
            set
            {
                _tRI = value;
                OnPropertyChanged("TRI");
            }
        }

        public double PNN50
        {
            get { return _pNN50; }
            set
            {
                _pNN50 = value;
                OnPropertyChanged("PNN50");
            }
        }

        public double SDNN
        {
            get { return _sDNN; }
            set
            {
                _sDNN = value;
                OnPropertyChanged("SDNN");
            }
        }

        public double SDSD
        {
            get { return _sDSD; }
            set
            {
                _sDSD = value;
                OnPropertyChanged("SDSD");
            }
        }

        public string ActivityID
        {
            get { return _activityID; }
            set
            {
                _activityID = value;
                OnPropertyChanged("ActivityID");
            }
        }

        public int StressIndex
        {
            get { return _stressIndex; }
            set
            {
                _stressIndex = value;
                OnPropertyChanged("StressIndex");
            }
        }

        public int IsStressed
        {
            get { return _isStressed; }
            set
            {
                _isStressed = value;
                OnPropertyChanged("IsStressed");
            }
        }

        public double GPSLat
        {
            get { return _gPSLat; }
            set
            {
                _gPSLat = value;
                OnPropertyChanged("GPSLat");
            }
        }

        public double GPSLng
        {
            get { return _gPSLng; }
            set
            {
                _gPSLng = value;
                OnPropertyChanged("GPSLng");
            }
        }



        public ICommand AddMeasurement
        {
            get
            {
                return _addMeasurement = _addMeasurement ?? new Command(async () =>
                {
                    var newMeasurement = new Measurements
                    {
                        UserID = UserID,
                        Date = Date,
                        TRI = TRI,
                        PNN50 = PNN50,
                        SDNN = SDNN,
                        SDSD = SDSD,
                        ActivityID = ActivityID,
                        StressIndex = StressIndex,
                        IsStressed = IsStressed,
                        GPSLat = GPSLat,
                        GPSLng = GPSLng
                    };

                    MeasurementsObj.Add(newMeasurement);
                    await _azureDataService.AddMeasurement(newMeasurement);
                });
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
