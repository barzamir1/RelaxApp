using App1.DataObjects;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App1.ViewModels
{
    public class MeasurementsPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Measurements> _measurements;
        private ObservableCollection<Measurements> _filteredMeasurements;
        private List<String> _activities;
        private List<Activities> allActivities;
        private ICommand _addMeasurement;
        private AzureDataService _azureDataService = AzureDataService.Instance;

        public event PropertyChangedEventHandler PropertyChanged;
        private static MeasurementsPageViewModel _instance;

        public MeasurementsPageViewModel()
        {
            if (_instance == null)
            {
                MeasurementsObj = new ObservableCollection<Measurements>();
                FilteredMeasurementsObj = new ObservableCollection<Measurements>();
                _activities = new List<string>();
                //InitializeMeasurement();
            }
        }
        public static async Task<MeasurementsPageViewModel> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MeasurementsPageViewModel();
                await _instance.InitializeActivities();
                await _instance.InitializeMeasurement();
            }
            return _instance;

        }
        public async Task<bool> InitializeMeasurement()
        {
            MeasurementsObj.Clear();
            FilteredMeasurementsObj.Clear();

            var measurement = await _azureDataService.GetMeasurements();

            foreach (var measure in measurement)
            {
                var acName = allActivities.Where(item => item.Id == measure.ActivityID).ToList();
                if (acName != null && acName.Count>0)
                    measure.ActivityName = acName[0].Name;
                measure.LabelColor = measure.IsStressed > 0 ? "Red" : "Default";
                MeasurementsObj.Add(measure);
            }
            
            return true;
        }

        public async Task InitializeActivities()
        {
            Activities.Clear();
            allActivities = await _azureDataService._activities.ToListAsync();
            if (allActivities.Count == 0)
                allActivities = await _azureDataService.GetActivitiesList();
            foreach(Activities item in allActivities)
            {
                if (item.Name != null)
                    _activities.Add(item.Name);
            }
        }

        public static bool IsInitialized { get { return _instance != null; } }
        public ObservableCollection<Measurements> MeasurementsObj
        {
            get { return _measurements; }
            set
            {
                _measurements = value;
                OnPropertyChanged("Measurements");
            }
        }
        public ObservableCollection<Measurements> FilteredMeasurementsObj
        {
            get { return _filteredMeasurements; }
            set
            {
                _filteredMeasurements = value;
                OnPropertyChanged("FilteredMeasurements");
            }
        }
        public List<String> Activities
        {
            get { return _activities; }
            set
            {
                _activities = value;
                OnPropertyChanged("Activities");
            }
        }
        public void ConcatFiltered(List<Measurements> lst)
        {
            try
            {
                lst.ForEach(item =>
                _filteredMeasurements.Add(item));
            }
            catch { }
        }
        //public string UserID
        //{
        //    get { return UserID; }
        //    set
        //    {
        //        _userID = value;
        //        OnPropertyChanged("UserID");
        //    }
        //}

        //public DateTime Date
        //{
        //    get { return _date; }
        //    set
        //    {
        //        _date = value;
        //        OnPropertyChanged("Date");
        //    }
        //}

        //public int TRI
        //{
        //    get { return _tRI; }
        //    set
        //    {
        //        _tRI = value;
        //        OnPropertyChanged("TRI");
        //    }
        //}

        //public double PNN50
        //{
        //    get { return _pNN50; }
        //    set
        //    {
        //        _pNN50 = value;
        //        OnPropertyChanged("PNN50");
        //    }
        //}

        //public double SDNN
        //{
        //    get { return _sDNN; }
        //    set
        //    {
        //        _sDNN = value;
        //        OnPropertyChanged("SDNN");
        //    }
        //}

        //public double SDSD
        //{
        //    get { return _sDSD; }
        //    set
        //    {
        //        _sDSD = value;
        //        OnPropertyChanged("SDSD");
        //    }
        //}

        //public string ActivityID
        //{
        //    get { return _activityID; }
        //    set
        //    {
        //        _activityID = value;
        //        OnPropertyChanged("ActivityID");
        //    }
        //}

        //public int StressIndex
        //{
        //    get { return _stressIndex; }
        //    set
        //    {
        //        _stressIndex = value;
        //        OnPropertyChanged("StressIndex");
        //    }
        //}

        //public int IsStressed
        //{
        //    get { return _isStressed; }
        //    set
        //    {
        //        _isStressed = value;
        //        OnPropertyChanged("IsStressed");
        //    }
        //}

        //public double GPSLat
        //{
        //    get { return _gPSLat; }
        //    set
        //    {
        //        _gPSLat = value;
        //        OnPropertyChanged("GPSLat");
        //    }
        //}

        //public double GPSLng
        //{
        //    get { return _gPSLng; }
        //    set
        //    {
        //        _gPSLng = value;
        //        OnPropertyChanged("GPSLng");
        //    }
        //}



        //public ICommand AddMeasurement
        //{
        //    get
        //    {
        //        return _addMeasurement = _addMeasurement ?? new Command(async () =>
        //        {
        //            var newMeasurement = new Measurements
        //            {
        //                UserID = UserID,
        //                Date = Date,
        //                TRI = TRI,
        //                PNN50 = PNN50,
        //                SDNN = SDNN,
        //                SDSD = SDSD,
        //                ActivityID = ActivityID,
        //                StressIndex = StressIndex,
        //                IsStressed = IsStressed,
        //                GPSLat = GPSLat,
        //                GPSLng = GPSLng
        //            };

        //            MeasurementsObj.Add(newMeasurement);
        //            await _azureDataService.AddMeasurement(newMeasurement);
        //        });
        //    }
        //}

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
