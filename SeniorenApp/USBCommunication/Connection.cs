using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using Java.IO;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorenApp.USBCommunication
{
    internal class Connection
    {
        private UsbAccessory _Accessory;
        private UsbManager _Manager;
        private FileInputStream _InputStream;
        private FileOutputStream _OutputStream;

        private Action<byte[]> _OnDataReceived;

        public Connection(UsbAccessory accessory, UsbManager manager, Action<byte[]> onDataReceived)
        {
            _Accessory = accessory;
            _Manager = manager;

            _OnDataReceived += onDataReceived;

            Log.Info("Accessory", nameof(Connection) + " : " + "created.");

            OpenConnection();

            Task.Factory.StartNew(() => ReceiveData());

            Log.Info("Accessory", nameof(Connection) + " : " + "task created.");
        }

        public void AddToDataReceivedEvent(Action<byte[]> onDataReceived)
        {
            if (!_OnDataReceived.GetInvocationList().Contains(onDataReceived))
            {
                _OnDataReceived += onDataReceived;
            }
        }

        public void RemoveFromDataReceivedEvent(Action<byte[]> onDataReceived)
        {
            if (_OnDataReceived.GetInvocationList().Contains(onDataReceived))
            {
                _OnDataReceived -= onDataReceived;
            }
        }

        private void OpenConnection()
        {
            Log.Info("Accessory", nameof(OpenConnection) + " : " + "called.");

            ParcelFileDescriptor fileDescriptor = _Manager.OpenAccessory(_Accessory);

            Log.Info("Accessory", nameof(OpenConnection) + " : " + "fileDescriptor created.");

            if (fileDescriptor != null)
            {
                _InputStream = new FileInputStream(fileDescriptor.FileDescriptor);
                _OutputStream = new FileOutputStream(fileDescriptor.FileDescriptor);

                Log.Info("Accessory", nameof(OpenConnection) + " : " + "Streams retrieved.");
            }
        }

        public void SendData(byte[] data)
        {
            if (_OutputStream != null)
            {
                try
                {
                    Log.Info("Accessory", nameof(SendData) + " : " + "called.");

                    _OutputStream.Write(data);

                    Log.Info("Accessory", nameof(SendData) + " : " + Encoding.ASCII.GetString(data) + " sent.");
                }
                catch (Java.Lang.Exception ex)
                {
                    Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private void ReceiveData()
        {
            while (true)
            {
                if (_InputStream != null)
                {
                    try
                    {
                        Log.Info("Accessory", nameof(ReceiveData) + " : " + "called.");

                        var data = new byte[16384];

                        _InputStream.Read(data);

                        Log.Info("Accessory", nameof(ReceiveData) + " : " + Encoding.ASCII.GetString(data) + " received.");

                        _OnDataReceived(data);

                        Log.Info("Accessory", nameof(ReceiveData) + " : " + nameof(_OnDataReceived) + " called.");
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
                    }
                }
            }
        }
    }
}