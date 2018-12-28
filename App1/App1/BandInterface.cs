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
        Task<bool> ConnectToBand(BaseViewModel b);
        Task<int> getHR(BaseViewModel b);
        Task<int> getGSR(BaseViewModel b, int sec);
        Task<bool> readRRSensor(BaseViewModel b, int sec);
        List<int> GsrReadings();
        List<double> RRIntervalReadings();
        void ClearAllReadings();
    }
}