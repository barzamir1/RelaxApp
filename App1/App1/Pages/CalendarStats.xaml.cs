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
using dotMorten.Xamarin.Forms;
//using XLabs.Forms.Controls;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Services;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarStats : ContentPage
    {
        Microcharts.LineChart chart = new Microcharts.LineChart();
        Services.AzureDataService azure = Services.AzureDataService.Instance;
        public List<Measurements> allMeasurements;
        public List<Measurements> filteredMeasurements;
        public List<Activities> allActivities;
        public MeasurementsPageViewModel model;

        public CalendarStats()
        {
            InitializeComponent();

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

        //return only measurements taken on 'day'
        public List<Measurements> GetDailyMeasurements(DateTime day, bool stressedOnly)
        {
            try
            {
                MeasurementsPageViewModel model = (MeasurementsPageViewModel)BindingContext;
                var currDayMeasure = model.MeasurementsObj.Where(item =>
               item.Date.CompareTo(day) > 0 && //measurement is later than day at midnight
               item.Date.CompareTo(day.AddDays(1)) < 0)  //measurement is earlier than day+1 at midnight
               .OrderBy(item => item.Date);
                if (stressedOnly)
                    return currDayMeasure.Where(item => item.IsStressed == 1).ToList();
                else
                    return currDayMeasure.ToList();
            }
            catch { return new List<Measurements>(); }
        }

        private async Task Initialize()
        {
            BindingContext = await MeasurementsPageViewModel.GetInstance();
            model = ((MeasurementsPageViewModel)BindingContext);
            model.FilteredMeasurementsObj.Clear();
            setChart();
        }

        //open popup and let the user change Activity 
        private async void Activity_Clicked(object sender, EventArgs e)
        {
            Label labelActivityName = (Label)sender;
            var measurementID = ((Label)labelActivityName.Parent.FindByName("MeasurementID")).Text;
            Measurements m = filteredMeasurements.Find(item => item.Id == measurementID);
            model = (MeasurementsPageViewModel)BindingContext;
            MeasurePopup popupPage = new MeasurePopup(m, model.Activities);
            popupPage.CallbackEvent += Popup_Closed;

            await PopupNavigation.Instance.PushAsync(popupPage);
        }

        //update the ListView
        private void Popup_Closed(object sender, Activities e)
        {
            model = (MeasurementsPageViewModel)BindingContext;
            model.FilteredMeasurementsObj.Clear();
            model.ConcatFiltered(filteredMeasurements);
            if (e != null)
                model.Activities.Add(e.Name);
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            setChart();
        }
        private void StressedOnlySwitch_Toggled(object sender, ToggledEventArgs e)
        {
            setChart();
        }

    }

}