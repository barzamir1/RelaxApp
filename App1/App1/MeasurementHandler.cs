using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace App1
{
    class MeasurementHandler
    {
        private static int _testTime = 90; //90 seconds measurements
        public static double _stressIndex = 0;

        public static async Task<String> GetStressResult(int pseudo)
        {
            List<double> rrIntervals = null;
            if (pseudo < 0)
            {
                bool isBandConnected = DependencyService.Get<BandInterface>().ConnectToBand(null).Result;
                if (!isBandConnected)
                    return null; //can't connect to band
                await DependencyService.Get<BandInterface>().getRRIntervals(_testTime);
                rrIntervals = DependencyService.Get<BandInterface>().RRIntervalReadings();
            }
            String result = await AzureFunctionAddMeasurement(rrIntervals, pseudo);
            return result;
        }
        static async Task<String> AzureFunctionAddMeasurement(List<double> intervals, int isPseudo)
        {
            String AZURE_FUNCTION_URL = "https://stresscalculator.azurewebsites.net/api/AddMeasurement?code=yz6PuH0ISJTFL4BWtnUX32fkAnw3bqFHGfzRfnRbBgd5B/AEdljX6w==&";

            var httpClient = new HttpClient();
            Dictionary<string, Object> query = new Dictionary<string, Object>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("UserID=" + Login.Default.CurrentUser.id + "&");
            stringBuilder.Append("ActivityName=unspecified&");
            stringBuilder.Append("msDateTime=" + DateTime.Now.Ticks + "&");
            stringBuilder.Append("GPSLng=1.1&");
            stringBuilder.Append("GPSLat=1.2&");
            
            if(isPseudo==-1)
                stringBuilder.Append("intervalsArr=" + JsonConvert.SerializeObject(intervals.ToArray()));
            else if(isPseudo==0)
                stringBuilder.Append("intervalsArr=" + getPseudoResults(0));
            else
                stringBuilder.Append("intervalsArr=" + getPseudoResults(1));


            Uri uri = new Uri(AZURE_FUNCTION_URL + stringBuilder.ToString());
            try
            {
                String content = await httpClient.GetStringAsync(uri);
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }
        public static String getPseudoResults(int i) //i=0 - relax
        {
            if (i == 0)
                return "[0.5328207166164451,1.239335790816174,1.3546783873460009,1.0565589926883643,0.7123345179531938,0.5900813725839672,1.4832992825658637,0.9206621262037202,1.0059934163574469,1.1380599159344267,0.8722550701387585,1.2674974247754136,1.4309437287480957,0.5863860203167446,1.2718150840929936,1.3888529307196813,0.8019706665890936,0.7204618953368512,1.2137808813825033,1.2547409692246987,1.1043231267371194,0.5051108843165113,1.0849168992945457,0.8763520362143402,0.804285693959826,1.157767276919823,0.9231004615441152,1.329978270528959,0.9430829916526002,0.843880252626056,1.3164175772840991,0.7475865454543122,0.6955977391708501,0.7352056450712989,1.1601072932572323,0.923706134177064,0.6578955961640378,1.3633426069214423,0.6652195698859458,0.9139680971559322,0.9910470000085685,0.9363429004851143,0.5655614247658589,0.7261108403301025,1.4009255496430084,0.8372313045809033,1.46522860538323,1.3525367418622731,1.260036899328633,1.4294564505237193]";
            return "[0.8793759999999999,0.8793759999999999,0.8793759999999999,0.8296,0.929152,0.8793759999999999,0.8793759999999999,0.8793759999999999,0.8296,0.647088,0.49776,0.796416,0.813008,0.779824,0.680272,0.464576,0.6968639999999999,0.7300479999999999,0.6636799999999999,0.613904,0.564128,0.680272,0.680272,0.6968639999999999,0.713456,0.713456,0.7300479999999999,0.74664,0.74664,0.74664,0.7300479999999999,0.74664,0.713456,0.6968639999999999,0.680272,0.680272,0.6636799999999999,0.6636799999999999,0.613904,0.6636799999999999,0.713456,0.713456,0.7300479999999999,0.7632319999999999,0.7632319999999999,0.7632319999999999,0.779824,0.779824,0.7632319999999999,0.7632319999999999,0.74664,0.7632319999999999,0.74664,0.7300479999999999,0.713456,0.7300479999999999,0.7300479999999999,0.7300479999999999,0.74664,0.7632319999999999,0.74664,0.796416,0.8296,0.8296,0.862784,0.8461919999999999,0.813008,0.7632319999999999,0.74664,0.7300479999999999,0.74664,0.7632319999999999,0.74664,0.7632319999999999,0.779824,0.7632319999999999,0.779824,0.779824,0.796416,0.813008,0.813008,0.796416,0.813008,0.813008,0.7632319999999999,0.7632319999999999,0.7300479999999999,0.74664,0.7300479999999999,0.74664,0.7300479999999999,0.74664,0.74664,0.7632319999999999,0.796416,0.779824,0.7632319999999999,0.779824,0.74664,0.74664,0.7632319999999999,0.74664,0.7632319999999999,0.796416,0.7632319999999999,0.796416,0.779824,0.779824,0.796416,0.813008,0.796416,0.7632319999999999,0.7632319999999999,0.779824,0.813008,0.779824,0.7300479999999999,0.6636799999999999,0.680272]";
        }
        //public static double CalcStressIndex()
        //{
        //    bool isBandConnected = DependencyService.Get<BandInterface>().ConnectToBand(null).Result;
        //    if (!isBandConnected)
        //        return -1; //can't connect to band
        //    bool readingSucceed = DependencyService.Get<BandInterface>().getRRIntervals(_testTime).Result;
        //    List<double> rrIntervals = DependencyService.Get<BandInterface>().RRIntervalReadings();

        //    //call Azure Function to calculate the stress index from these readings
        //    _stressIndex = AzureFunc(rrIntervals);

        //    return _stressIndex; 
        //}
        /* NN50 is a random variable that measure heart rate variability (HRV)
         * it counts the number of following intervals that differ in more than 50ms
         * PNN50 is the proportion of NN50 divided by total number of intervals.
         * during stress, NN50 (and PNN50) should decrease.
         */
        //public static double AzureFunc(List<double> rr) //should not really be here
        //{
        //    int NN50;
        //    double PNN50; //the % of NN50 relative to all intervals

        //    List<double> diff = new List<double>(); //the difference between each two following intervals
        //    for(int i=0; i<rr.Count-1; i++)
        //    {
        //        diff.Add(Math.Abs(rr[i] - rr[i + 1])); //the absolute difference between two rr intervals
        //    }
        //    List<double> temp = diff.Where(item => item > 0.05).ToList();
        //    NN50 = temp.Count;
        //    PNN50 = ((double)NN50 / (double)rr.Count)*100; //we want percentage
        //    return PNN50;
        //}
    }
}
