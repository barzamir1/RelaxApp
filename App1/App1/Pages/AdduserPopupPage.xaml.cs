using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;
using App1.ViewModels;

namespace App1.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AdduserPopupPage : PopupPage
    {
		public AdduserPopupPage ()
		{
			InitializeComponent ();
		}

        private async void OnAdd(object sender, EventArgs e)
        {
            BindingContext = await UserAuthorizationModel.GetInstance();

            if (!ValidateUserCode()) { return; }
            var userShortID = entryUserCode.Text;
            buttonAddUser.IsEnabled = false;
            bool success = ((UserAuthorizationModel)BindingContext).AddAuthUser(userShortID);
            if (!success) { labelUserCodeError.Text = "Code Error: wrong user code"; }
            buttonAddUser.IsEnabled = true;

            await Navigation.PopAsync();
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
            if (input.Length != 8) //TODO: add class with const values
            {
                labelUserCodeError.Text = "Code Error: code should contain 8 letters and numbers";
                return false;
            }
            if (Regex.IsMatch(input, "^[a-z0-9]*$"))
            {
                labelUserCodeError.Text = "";
                entryUserCode.Text = input;
                return true;
            }
            labelUserCodeError.Text = "Code Error: code should contain 8 letters and numbers";
            return false;
        }

    }
}