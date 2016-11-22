using Android.Hardware.Usb;
using SeniorenApp.USBCommunication;
using System;

namespace SeniorenApp.Helper
{
    internal static class USBHelper
    {
        private static Connection _USBConnection;

        public static Connection USBConnection
        {
            get { return _USBConnection; }
        }

        public static void CreateUSBConnection(UsbAccessory accessory, UsbManager manager, Action<byte[]> onDataReceived)
        {
            if (_USBConnection == null)
            {
                _USBConnection = new Connection(accessory, manager, onDataReceived);
            }
        }
    }
}