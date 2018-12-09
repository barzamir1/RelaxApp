using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

/*
 * this class is only used for Debugging.
 * it displays the HR and GSR measurements in the UI.
 * -BaseViewModel updates the controls in MainPage.xaml when one of its properties changes.
 * -DependencyService.Get<BandInterface>().func() is the way we use an android-only class in Xamarin
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
            //this.buttonConnect.IsEnabled = !isConnected.Result;
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
            Task<int> o = DependencyService.Get<BandInterface>().getGSR(b, 3);
        }

        private async void ButtonGSRWindow_Clicked(object sender, EventArgs e)
        {
            BaseViewModel b = (BaseViewModel)BindingContext;
            b.GSR = 0;
            this.buttonGSRWindow.IsEnabled = false;
            await DependencyService.Get<BandInterface>().getGSR(b,40);
            //List<int> windowReads = DependencyService.Get<BandInterface>().getList();
            this.buttonGSRWindow.IsEnabled = true;
        }
    }
}
