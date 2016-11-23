using Android.Hardware.Usb;
using Android.OS;
using Java.IO;
using SeniorenApp.Helper;
using System;
using System.Linq;
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
            Logger.LogInfo(nameof(Connection) + " - Constructor", "called.");

            _Accessory = accessory;
            _Manager = manager;

            _OnDataReceived += onDataReceived;

            OpenConnection();

            Task.Factory.StartNew(() => ReceiveData());

            Logger.LogInfo(nameof(Connection) + " - Constructor", nameof(ReceiveData) + " task : " + "created.");
        }

        public void AddToDataReceivedEvent(Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(Connection) + " - " + nameof(AddToDataReceivedEvent), "called.");

            if (!_OnDataReceived.GetInvocationList().Contains(onDataReceived))
            {
                Logger.LogInfo(nameof(Connection) + " - " + nameof(AddToDataReceivedEvent), "invocationlist did not contain " + nameof(onDataReceived));

                _OnDataReceived += onDataReceived;
            }
        }

        public void RemoveFromDataReceivedEvent(Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(Connection) + " - " + nameof(RemoveFromDataReceivedEvent), "called.");

            if (_OnDataReceived.GetInvocationList().Contains(onDataReceived))
            {
                Logger.LogInfo(nameof(Connection) + " - " + nameof(RemoveFromDataReceivedEvent), "invocationlist did contain " + nameof(onDataReceived));

                _OnDataReceived -= onDataReceived;
            }
        }

        private void OpenConnection()
        {
            Logger.LogInfo(nameof(Connection) + " - " + nameof(OpenConnection), "called.");

            ParcelFileDescriptor fileDescriptor = _Manager.OpenAccessory(_Accessory);

            Logger.LogInfo(nameof(Connection) + " - " + nameof(OpenConnection), "accessory opened." + nameof(fileDescriptor) + " created.");

            if (fileDescriptor != null)
            {
                Logger.LogInfo(nameof(Connection) + " - " + nameof(OpenConnection), nameof(fileDescriptor) + " was not null.");

                _InputStream = new FileInputStream(fileDescriptor.FileDescriptor);
                _OutputStream = new FileOutputStream(fileDescriptor.FileDescriptor);

                Logger.LogInfo(nameof(Connection) + " - " + nameof(OpenConnection), "Streams retrieved.");
            }
        }

        public void SendData(byte[] data)
        {
            Logger.LogInfo(nameof(Connection) + " - " + nameof(SendData), "called."); 

            if (_OutputStream != null)
            {
                Logger.LogInfo(nameof(Connection) + " - " + nameof(SendData), nameof(_OutputStream) + " was not null.");

                try
                {
                    _OutputStream.Write(data);

                    Logger.LogInfo(nameof(Connection) + " - " + nameof(SendData), data.Length + " bytes sent. Message: " + BitConverter.ToString(data));
                }
                catch (Java.Lang.Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void ReceiveData()
        {
            Logger.LogInfo(nameof(Connection) + " - " + nameof(ReceiveData), "called.");

            while (true)
            {
                if (_InputStream != null)
                {
                    Logger.LogInfo(nameof(Connection) + " - " + nameof(ReceiveData), nameof(_InputStream) + " was not null.");

                    try
                    {
                        var data = new byte[16384];

                        _InputStream.Read(data);

                        Logger.LogInfo(nameof(Connection) + " - " + nameof(ReceiveData), data.Length + " bytes received. Message: " + BitConverter.ToString(data));                        

                        _OnDataReceived(data);

                        Logger.LogInfo(nameof(Connection) + " - " + nameof(ReceiveData), nameof(_OnDataReceived) + " called.");
                    }
                    catch (Java.Lang.Exception ex)
                    {
                        Logger.LogError(ex);
                    }
                }
            }
        }
    }
}