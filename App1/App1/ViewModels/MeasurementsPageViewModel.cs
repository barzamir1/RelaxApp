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

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
