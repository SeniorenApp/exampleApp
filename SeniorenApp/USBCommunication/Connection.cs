using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using Java.IO;
using System;

namespace SeniorenApp.USBCommunication
{
    internal class Connection
    {
        private CommunicationHandler _UsbHandler;

        private UsbAccessory _Accessory;
        private UsbManager _Manager;
        private Action<byte[]> _OnDataReceived;

        public Connection(UsbAccessory accessory, UsbManager manager, Action<byte[]> onDataReceived)
        {
            _Accessory = accessory;
            _Manager = manager;
            _OnDataReceived = onDataReceived;

            Log.Info("Accessory", nameof(Connection) + " : " + "created.");
        }

        public void OpenConnection()
        {
            Log.Info("Accessory", nameof(OpenConnection) + " : " + "called.");

            ParcelFileDescriptor fileDescriptor = _Manager.OpenAccessory(_Accessory);

            Log.Info("Accessory", nameof(OpenConnection) + " : " + "fileDescriptor created.");

            if (fileDescriptor != null)
            {
                var inputStream = new FileInputStream(fileDescriptor.FileDescriptor);
                var outputStream = new FileOutputStream(fileDescriptor.FileDescriptor);

                Log.Info("Accessory", nameof(OpenConnection) + " : " + "Streams retrieved.");

                _UsbHandler = new CommunicationHandler(inputStream, outputStream, _OnDataReceived);

                Log.Info("Accessory", nameof(OpenConnection) + " : " + "UsbHandler created.");
            }
        }

        public void SendData(byte[] data)
        {
            _UsbHandler.SendData(data);
        }
    }
}