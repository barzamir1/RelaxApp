using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.DataObjects;
using SkiaSharp;
using App1.ViewModels;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarStats : ContentPage
    {
        Microcharts.LineChart chart = new Microcharts.LineChart();
        Services.AzureDataService azure = Services.AzureDataService.Instance;
        public List<Measurements> allMeasurements;
        public List<Measurements> filteredMeasurements;
        public MeasurementsPageViewModel model;

        public CalendarStats()
        {
            InitializeComponent();
            BindingContext = new MeasurementsPageViewModel();
            datePicker.Date = DateTime.Today;
            datePicker.MaximumDate = DateTime.Now;

            dateChart.Chart = chart;
            Initialize();
        }
        public void setChart()
        {
            List<Microcharts.Entry> entries = new List<Microcharts.Entry>();

            //var selectedDay = calendar.SelectedDate.Value;
            var selectedDay = datePicker.Date;
            bool stressedOnly = stressedOnlySwitch.IsToggled;
            filteredMeasurements = GetDailyMeasurements(selectedDay, stressedOnly);
            filteredMeasurements.ForEach(item =>
            {
                var entry = new Microcharts.Entry(item.StressIndex)
                {
                    Color = item.IsStressed == 1 ? SKColor.Parse("#cc1b08") : SKColor.Parse("#08d854"),
                    Label = item.Date.ToString("HH:mm"),
                    ValueLabel = item.StressIndex.ToString()
                };
                entries.Add(entry);
            });
            chart.Entries = entries;
            chart.ValueLabelOrientation = Microcharts.Orientation.Horizontal;
            model = ((MeasurementsPageViewModel)BindingContext);
            model.FilteredMeasurementsObj.Clear();
            model.ConcatFiltered(filteredMeasurements);
        }
        public List<Measurements> GetDailyMeasurements(DateTime day, bool stressedOnly)
        {
            try
            {
                var currDayMeasure = allMeasurements.Where(item =>
               item.Date.CompareTo(day) > 0 && //measurement is later than day at midnight
               item.Date.CompareTo(day.AddDays(1)) < 0);  //measurement is earlier than day+1 at midnight
                if (stressedOnly)
                    return currDayMeasure.Where(item => item.IsStressed == 1).ToList();
                else
                    return currDayMeasure.ToList();
            }
            catch { return new List<Measurements>(); }
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            setChart();
        }
        private void StressedOnlySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            setChart();
        }

        private async void Initialize()
        {
            var m = await azure.GetMeasurements();
            allMeasurements = m.ToList();
            var activities = await azure.GetActivitiesList();
            foreach (var measure in allMeasurements)
            {
                var name = activities.Find(item => item.Id == measure.ActivityID);
                if (name != null)
                    measure.ActivityName = name.Name;
                measure.LabelColor = measure.IsStressed > 0 ? "Red" : "Default";
            }
        }
    }
}