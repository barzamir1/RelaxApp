using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly: Dependency(typeof(App1.Bluetooth))]
namespace App1
{
    class Bluetooth : IBluetooth
    {
        //returns the current state of the Bluetooth adapter
        public bool TurnOff()
        {
            BluetoothAdapter b = BluetoothAdapter.DefaultAdapter;
            if (!b.IsEnabled)
                return false; //Bluetooth was off
            b.Disable();
            return true;
        }

        public bool TurnOn()
        {
            BluetoothAdapter b = BluetoothAdapter.DefaultAdapter;
            if (b.IsEnabled)
                return true;
            b.Enable();
            return false;
        }
    }
}