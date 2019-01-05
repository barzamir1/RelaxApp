//using Microsoft.Band;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace App1
{
    public interface BandInterface
    {
        Task<bool> ConnectToBand(TestMeViewModel b);
        Task<int> getHR(TestMeViewModel b);
        Task<int> getGSR(TestMeViewModel b, int sec);
        Task<bool> readRRSensor(TestMeViewModel b, int sec);
        List<int> GsrReadings();
        List<double> RRIntervalReadings();
        void ClearAllReadings();
    }
}