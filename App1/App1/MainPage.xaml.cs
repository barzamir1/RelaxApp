using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

/*
 *this class is only used for Debugging.
 * it is used to display HR and GSR measurements in the UI.
 *-BaseViewModel updates the controls in MainPage.xaml when one of its properties changes.
 *-DependencyService.Get<BandInterface>().func() is the way we use an android-only class in Xamarin
 * we created an Interface BandInterface and implemented the functions in Band.cs (android-class)
 */
namespace App1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new BaseViewModel();
        }

        private void ButtonConnect_Clicked(object sender, EventArgs e)
        {
            BaseViewModel b = (BaseViewModel)BindingContext;
           
            Task<bool> isConnected = DependencyService.Get<BandInterface>().ConnectToBand(b);
            this.buttonConnect.IsEnabled = false;
        }

        private void ButtonHR_Clicked(object sender, EventArgs e)
        {
            BaseViewModel b = (BaseViewModel)BindingContext;
            b.HR = 0;
            Task<int> o = DependencyService.Get<BandInterface>().getHR(b);
        }

        private void ButtonGSR_Clicked(object sender, EventArgs e)
        {
            BaseViewModel b = (BaseViewModel)BindingContext;
            b.GSR = 0;
            Task<int> o = DependencyService.Get<BandInterface>().getGSR(b);
        }
    }
}
