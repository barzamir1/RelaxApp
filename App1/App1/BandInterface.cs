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
        Task<int> getGSR(BaseViewModel b);
    }
}