using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App1
{
    class StressCalculator
    {
        private static int _testTime = 90; //90 seconds measurements
        public static double _stressIndex = 0;

        public static double CalcStressIndex()
        {
            bool isBandConnected = DependencyService.Get<BandInterface>().ConnectToBand(null).Result;
            if (!isBandConnected)
                return -1; //can't connect to band
            bool readingSucceed = DependencyService.Get<BandInterface>().getRRIntervals(_testTime).Result;
            List<double> rrIntervals = DependencyService.Get<BandInterface>().RRIntervalReadings();
            
            //call Azure Function to calculate the stress index from these readings
            _stressIndex = AzureFunc(rrIntervals);

            return _stressIndex; 
        }
        /* NN50 is a random variable that measure heart rate variability (HRV)
         * it counts the number of following intervals that differ in more than 50ms
         * PNN50 is the proportion of NN50 divided by total number of intervals.
         * during stress, NN50 (and PNN50) should decrease.
         */
        public static double AzureFunc(List<double> rr) //should not really be here
        {
            int NN50;
            double PNN50; //the % of NN50 relative to all intervals

            List<double> diff = new List<double>(); //the difference between each two following intervals
            for(int i=0; i<rr.Count-1; i++)
            {
                diff.Add(Math.Abs(rr[i] - rr[i + 1])); //the absolute difference between two rr intervals
            }
            List<double> temp = diff.Where(item => item > 0.05).ToList();
            NN50 = temp.Count;
            PNN50 = ((double)NN50 / (double)rr.Count)*100; //we want percentage
            return PNN50;
        }
    }
}
