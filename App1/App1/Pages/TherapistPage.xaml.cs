using App1.DataObjects;
using App1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TherapistPage : ContentPage
	{
		public TherapistPage ()
		{
			InitializeComponent ();
            InitTherapist();
        }
        private async void InitTherapist()
        {
            BindingContext = await UserAuthorizationModel.GetInstance();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedUserID = ((Users)allowedUsersListView.SelectedItem).id;
            Login.Default.CurrentUser.WatchingUserID = selectedUserID;
            await Navigation.PushAsync(new StatsTabbedPage());
        }
    }
}