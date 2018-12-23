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
    class ActivitiesPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Activities> _activities;
        private string _name;
        private int _counter;
        private ICommand _addActivity;
        private AzureDataService _azureDataService = new AzureDataService();

        public event PropertyChangedEventHandler PropertyChanged;

        public ActivitiesPageViewModel()
        {
            ActivitiesObj = new ObservableCollection<Activities>();

            InitializeActivity();
        }

        private async void InitializeActivity()
        {
            var activity = await _azureDataService.GetActivities();

            foreach (var activity1 in activity)
            {
                ActivitiesObj.Add(activity1);
            }
        }

        public ObservableCollection<Activities> ActivitiesObj
        {
            get { return _activities; }
            set
            {
                _activities = value;
                OnPropertyChanged("Activities");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public int Counter
        {
            get { return _counter; }
            set
            {
                _counter = value;
                OnPropertyChanged("Counter");
            }
        }


        public ICommand AddActivity
        {
            get
            {
                return _addActivity = _addActivity ?? new Command(async () =>
                {
                    var newActivity = new Activities
                    {

                        Name = Name,
                        /* set new activity counter to 1 */
                        Counter = 1
                    };

                    ActivitiesObj.Add(newActivity);
                    await _azureDataService.AddActivity(newActivity);
                });
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
