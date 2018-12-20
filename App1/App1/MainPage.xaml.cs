using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

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

        private void ButtonRRInterval_Clicked(object sender, EventArgs e)
        {
            //this.buttonGSRWindow.IsEnabled = false;
            BaseViewModel b = (BaseViewModel)BindingContext;
            b.PNN50 = 0;
            double PNN50 = 0;
            Action onCompleted = () =>
            {
                b.PNN50 = PNN50;
            };
            //don't make the UI thread wait
            var thread = new Thread(
              () =>
              {
                  try
                  {
                     PNN50 = StressCalculator.CalcStressIndex();
                  }
                  finally
                  {
                      onCompleted();
                  }
              });
            thread.Start();
        }

        private async void ButtonAzureFunctionInvoke(object sender, EventArgs e)
        {
            String AZURE_FUNCTION_URL = "https://stresscalculator.azurewebsites.net/api/AddMeasurement?code=yz6PuH0ISJTFL4BWtnUX32fkAnw3bqFHGfzRfnRbBgd5B/AEdljX6w==&";
            var httpClient = new HttpClient();
            Dictionary<string, Object> query = new Dictionary<string, Object>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("UserID=1234&");
            stringBuilder.Append("ActivityName=talking&");
            stringBuilder.Append("msDateTime="+DateTime.Now.Ticks+"&");
            stringBuilder.Append("GPSLng=1.1&");
            stringBuilder.Append("GPSLat=1.2&");
            stringBuilder.Append("intervalsArr="+getArr(1));
            Uri uri = new Uri(AZURE_FUNCTION_URL +stringBuilder.ToString());
            try
            {
                var content = await httpClient.GetStringAsync(uri);
                var result = JsonConvert.DeserializeObject(content);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public String getArr(int i) //i=0 - relax
        {
            if(i==0)
                return "[0.5328207166164451,1.239335790816174,1.3546783873460009,1.0565589926883643,0.7123345179531938,0.5900813725839672,1.4832992825658637,0.9206621262037202,1.0059934163574469,1.1380599159344267,0.8722550701387585,1.2674974247754136,1.4309437287480957,0.5863860203167446,1.2718150840929936,1.3888529307196813,0.8019706665890936,0.7204618953368512,1.2137808813825033,1.2547409692246987,1.1043231267371194,0.5051108843165113,1.0849168992945457,0.8763520362143402,0.804285693959826,1.157767276919823,0.9231004615441152,1.329978270528959,0.9430829916526002,0.843880252626056,1.3164175772840991,0.7475865454543122,0.6955977391708501,0.7352056450712989,1.1601072932572323,0.923706134177064,0.6578955961640378,1.3633426069214423,0.6652195698859458,0.9139680971559322,0.9910470000085685,0.9363429004851143,0.5655614247658589,0.7261108403301025,1.4009255496430084,0.8372313045809033,1.46522860538323,1.3525367418622731,1.260036899328633,1.4294564505237193]";
            return "[0.8793759999999999,0.8793759999999999,0.8793759999999999,0.8296,0.929152,0.8793759999999999,0.8793759999999999,0.8793759999999999,0.8296,0.647088,0.49776,0.796416,0.813008,0.779824,0.680272,0.464576,0.6968639999999999,0.7300479999999999,0.6636799999999999,0.613904,0.564128,0.680272,0.680272,0.6968639999999999,0.713456,0.713456,0.7300479999999999,0.74664,0.74664,0.74664,0.7300479999999999,0.74664,0.713456,0.6968639999999999,0.680272,0.680272,0.6636799999999999,0.6636799999999999,0.613904,0.6636799999999999,0.713456,0.713456,0.7300479999999999,0.7632319999999999,0.7632319999999999,0.7632319999999999,0.779824,0.779824,0.7632319999999999,0.7632319999999999,0.74664,0.7632319999999999,0.74664,0.7300479999999999,0.713456,0.7300479999999999,0.7300479999999999,0.7300479999999999,0.74664,0.7632319999999999,0.74664,0.796416,0.8296,0.8296,0.862784,0.8461919999999999,0.813008,0.7632319999999999,0.74664,0.7300479999999999,0.74664,0.7632319999999999,0.74664,0.7632319999999999,0.779824,0.7632319999999999,0.779824,0.779824,0.796416,0.813008,0.813008,0.796416,0.813008,0.813008,0.7632319999999999,0.7632319999999999,0.7300479999999999,0.74664,0.7300479999999999,0.74664,0.7300479999999999,0.74664,0.74664,0.7632319999999999,0.796416,0.779824,0.7632319999999999,0.779824,0.74664,0.74664,0.7632319999999999,0.74664,0.7632319999999999,0.796416,0.7632319999999999,0.796416,0.779824,0.779824,0.796416,0.813008,0.796416,0.7632319999999999,0.7632319999999999,0.779824,0.813008,0.779824,0.7300479999999999,0.6636799999999999,0.680272]";
        }
    }
}
