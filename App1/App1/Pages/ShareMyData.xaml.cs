using App1.DataObjects;
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
	public partial class ShareMyData : ContentPage
	{
		public ShareMyData ()
		{
			InitializeComponent ();
            Users currUser = Login.Default.CurrentUser;
            if (currUser != null)
                labelUserShortId.Text = currUser.shortID.Insert(4, "-");
		}
	}
}