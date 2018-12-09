using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Xamarin.Forms;
using System.Windows.Input;

namespace App1
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _hr = 0;
        private int _gsr = 0;
        private String _gsrListStr = "";
        private bool _isConnected = false;        
        
        public int HR
        {
            get { return _hr; }
            set
            {
                if (HR != value)
                {
                    _hr = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HR"));
                }
            }
        }
        public int GSR
        {
            get { return _gsr; }
            set
            {
                if (GSR != value)
                {
                    _gsr = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GSR"));
                }
            }
        }
        public String GsrList
        {
            get { return _gsrListStr; }
            set
            {
                if (value.Equals("#"))
                {
                    _gsrListStr = "";
                    return;
                }
                if (!GsrList.Equals(value))
                {
                    _gsrListStr = GsrList + value + ", ";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GsrList"));
                }
            }
        }
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (IsConnected != value)
                {
                    _isConnected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
                }
            }
        }
        public void ClearGsr()
        {
            _gsrListStr = "";
        }
    }
}
