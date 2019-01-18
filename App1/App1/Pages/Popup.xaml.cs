using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using App1.DataObjects;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using App1.Services;
using System.Collections;
using dotMorten.Xamarin.Forms;
using System.Text.RegularExpressions;
using System.Linq;

namespace App1.Pages
{
    public partial class MeasurePopup : PopupPage
    {
        //public object FrameContainer { get; private set; }
        Measurements measurement;
        List<String> activities;
        public event EventHandler<Activities> CallbackEvent;

        public MeasurePopup()
        {
            InitializeComponent();
            this.IsAnimationEnabled = false;
        }

        public MeasurePopup(Measurements measurement, List<String> activities)
        {
            InitializeComponent();
            this.IsAnimationEnabled = false;
            this.measurement = measurement;
            this.activities = activities;

            labelDate.Text = measurement.Date.ToString("dd/MM/yy HH:mm");
            labelStressLevel.Text = "Stress Level: "+measurement.StressIndex.ToString();
            labelStressLevel.TextColor = measurement.IsStressed > 0 ? Color.Red : Color.Default;
            autoSuggestionBox.PlaceholderText = measurement.ActivityName;
            autoSuggestionBox.ItemsSource = activities;
        }

        //set the suggestions
        private void AutoSuggestionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                String pattern = autoSuggestionBox.Text.ToLower();
                var filteredActivities = activities.Where(x => x != null && x.ToLower().StartsWith(pattern))
                    .Concat(activities.Where(x => x != null && !x.ToLower().StartsWith(pattern) && x.ToLower().Contains(pattern)))
                    .ToList();

                sender.ItemsSource = filteredActivities;
            }
        }

        //close popup and update measurement's activity id
        private async void OnCloseButtonTapped(object sender, EventArgs e)
        {
            bool isNewActivity = false;
            if (autoSuggestionBox.Text != "")
            {
                measurement.ActivityName = autoSuggestionBox.Text;
                var azure = AzureDataService.Instance;
                var temp = await azure._activities.Where(item => item.Name == autoSuggestionBox.Text).Take(1).ToListAsync();
                Activities newActivity = temp.Count > 0 ? temp[0] : null;
                if (newActivity == null)
                {
                    newActivity = new Activities();
                    newActivity.Name = autoSuggestionBox.Text;
                    await azure.AddActivity(newActivity);
                    azure.SyncActivties(); //doesn't wait
                    isNewActivity = true;
                }
                measurement.ActivityID = newActivity.Id;
                azure.UpdateMeasurement(measurement); //doesn't wait

                if (isNewActivity)
                    CallbackEvent.Invoke(this, newActivity);
                else
                    CallbackEvent.Invoke(this, null);
            }
            CloseAllPopup();
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
        protected override bool OnBackgroundClicked()
        {
            //OnCloseButtonTapped(null,null);
            return false;
        }
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }
        protected override async Task OnAppearingAnimationEndAsync()
        {

        }
        protected override async Task OnDisappearingAnimationBeginAsync()
        {

        }
    }
}