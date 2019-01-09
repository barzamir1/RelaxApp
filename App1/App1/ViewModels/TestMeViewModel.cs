using System;
using System.ComponentModel;

namespace App1
{
    public class TestMeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _hr = 0;
        private int _gsr = 0;
        private String _gsrListStr = "";
        private double _PNN50 = 0;
        private bool _isConnected = false;
        private double _progress = 1;
        private String  _stressResult="";
        private bool _isFinished = true;

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
        public double PNN50
        {
            get { return _PNN50; }
            set
            {
                if (PNN50 != value)
                {
                    _PNN50 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PNN50"));
                }
            }
        }
        public double Progress
        {
            get { return _progress; }
            set
            {
                if (Progress != value)
                {
                    _progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Progress"));
                }
            }
        }
        public String StressResult
        {
            get { return _stressResult; }
            set
            {
                if (StressResult != value)
                {
                    _stressResult = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StressResult"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsFinished"));
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
        public bool IsFinished
        {
            get { return _progress == 1; }
        }

        public void ClearGsr()
        {
            _gsrListStr = "";
        }
    }
}
