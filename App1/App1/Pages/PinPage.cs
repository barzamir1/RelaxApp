using Android;
using App1.DataObjects;
using App1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace WorkingWithMaps
{
    public class PinPage : ContentPage
    {
        Map map;
        public List<Measurements> filteredMeasurements;
        readonly string[] PermissionsLocation =
        {
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
        };
        const int RequestLocationId = 0;


        public PinPage()
        {
            map = new Map
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            Initialize();

            // put the page together
            Content = new StackLayout
            {
                Spacing = 0,
                Children = {
                    map
                }
            };
        }
        public async void Initialize()
        {
            await GetMeasurements();
            SetPins();
        }
        public async Task GetMeasurements()
        {
            MeasurementsPageViewModel model = await MeasurementsPageViewModel.GetInstance();

            DateTime weekAgo = DateTime.Today.AddDays(-7);
            filteredMeasurements = model.MeasurementsObj
                .Where(item => item.Date.CompareTo(weekAgo) > 0) //measurements from last week
                .Where(item => item.IsStressed>0) //show only stressed measurements
                .ToList();
        }
        public void SetPins()
        {
            map.Pins.Clear();
            filteredMeasurements.ForEach(measure =>
            {
                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(measure.GPSLat, measure.GPSLng),
                    Label = measure.Date.ToString("dd/MM HH:mm"),
                    Address = "Activity: " + measure.ActivityName
                };
                map.Pins.Add(pin);
            });
            if (filteredMeasurements.Count > 0)
            {
                //focus map on last measurement
                int last = filteredMeasurements.Count - 1;
                map.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Position(filteredMeasurements[last].GPSLat, filteredMeasurements[last].GPSLng), Distance.FromMiles(1.5)));

            }
        }
    }
}

