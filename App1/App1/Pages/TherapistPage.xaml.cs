using App1.DataObjects;
using App1.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Text.RegularExpressions;

using Xamarin.Forms;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TherapistPage : ContentPage
	{
        bool firstUserClicked = true;
        private AdduserPopupPage _adduserPopup;

        public TherapistPage ()
		{
			InitializeComponent ();
            InitTherapist();
            _adduserPopup = new AdduserPopupPage();
        }

        protected override void OnAppearing()
        {
            InitTherapist();
            _adduserPopup = new AdduserPopupPage();
        }

        private async void InitTherapist()
        {
            BindingContext = await UserAuthorizationModel.GetInstance();
        }

        private async void OnOpenPupup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(_adduserPopup);
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedUser = (Users)allowedUsersListView.SelectedItem;
            if (selectedUser != null)
            {
                Login.Default.CurrentUser.WatchingUserID = selectedUser.id;

                if (!firstUserClicked)
                {
                    //get the newly selected user's measurements
                    var model = await MeasurementsPageViewModel.GetInstance();
                    await model.InitializeMeasurement();
                }
                firstUserClicked = false;
                await Navigation.PushAsync(new StatsTabbedPage());
                allowedUsersListView.SelectedItem = null;
            }
        }
    
        /* moved to popup
         * 
        private void AddUser_Clicked(object sender, EventArgs e)
        {
            if (!ValidateUserCode()) { return; }
            var userShortID = entryUserCode.Text;
            buttonAddUser.IsEnabled = false;
            bool success = ((UserAuthorizationModel)BindingContext).AddAuthUser(userShortID);
            if (!success) { labelUserCodeError.Text = "wrong user code"; }
            buttonAddUser.IsEnabled = true;

        }

        private bool ValidateUserCode()
        {
            var input = entryUserCode.Text;
            if (input == null || input.Length < 1)
            {
                labelUserCodeError.Text = "enter user's code";
                return false;
            }
            input = input.Replace("-", "").ToLower();
            if (input.Length!=8) //TODO: add class with const values
            {
                labelUserCodeError.Text = "code should contain 8 letters and numbers";
                return false;
            }
            if (Regex.IsMatch(input, "^[a-z0-9]*$"))
            {
                labelUserCodeError.Text = "";
                entryUserCode.Text = input;
                return true;
            }
            labelUserCodeError.Text = "code should contain 8 letters and numbers";
            return false;
        }
        */
    }
}