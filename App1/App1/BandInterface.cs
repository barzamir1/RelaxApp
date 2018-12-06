//using Microsoft.Band;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;


namespace App1
{
    public class BandModel : BaseViewModel
    {

        //static BandDeviceInfo _selectedBand;
        //static IBandInfo _selectedBand;

        // public static IBandInfo SelectedBand //IBandInfo
        // {
        //     get { return _selectedBand; }
        //     set { _selectedBand = value; }
        // }

        // private static IBandClient _bandClient;
        //// private static BandClient _bandClient;
        // public static IBandClient BandClient //IBandClient
        // {
        //     get { return _bandClient; }
        //     set
        //     {
        //         _bandClient = value;
        //     }
        // }


        // public static bool IsConnected
        // {
        //     get
        //     {
        //         return BandClient != null;
        //     }

        // }

        // public static async Task FindDevicesAsync()
        // {

        //     var bands = await BandClientManaget.
        //     if (bands != null && bands. > 0)
        //     {
        //         SelectedBand = bands[0]; // take the first band

        //     }
        // }
    }

    public interface BandInterface
    {
        Task<bool> ConnectToBand(BaseViewModel b);
        Task<int> getHR(BaseViewModel b);
        Task<int> getGSR(BaseViewModel b);
    }
}