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
	public partial class LoadingAnimation : ContentPage
	{
		public LoadingAnimation ()
		{
			InitializeComponent ();
            activityIndicator.IsRunning = true;
		}
        public void Complete()
        {
            activityIndicator.IsRunning = false;
        }
	}
}