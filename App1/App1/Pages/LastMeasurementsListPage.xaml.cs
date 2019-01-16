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
	public partial class LastMeasurementsListPage : ContentPage
	{
		public LastMeasurementsListPage ()
		{
			InitializeComponent();
            BindingContext = new ViewModels.MeasurementsPageViewModel();
            ((ViewModels.MeasurementsPageViewModel)BindingContext).InitializeMeasurement();
		}
	}
}