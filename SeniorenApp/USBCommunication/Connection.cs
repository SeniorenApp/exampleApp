using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Java.IO;
using SeniorenApp.Helper;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeniorenApp.USBCommunication
{
    internal class Connection
    {
        private const int HEADER_LENGTH = 2;

        private UsbAccessory _Accessory;
        private UsbManager _Manager;
        private FileInputStream _InputStream;
        private FileOutputStream _OutputStream;
        private CancellationTokenSource _TaskCancelToken;

        private Action<byte[]> _OnDataReceived;

        public Connection(UsbAccessory accessory, UsbManager manager, Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(Connection), "Constructor", "called.");

            try
            {
                _Accessory = accessory;
                _Manager = manager;

                _OnDataReceived += onDataReceived;

                OpenConnection();

                _TaskCancelToken = new CancellationTokenSource();
                var cancelToken = _TaskCancelToken.Token;

                Task.Factory.StartNew(() => ReceiveData(), cancelToken);

                Logger.LogInfo(nameof(Connection), "Constructor", nameof(ReceiveData) + " task : " + "created.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }        

        public void AddToDataReceivedEvent(Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(Connection), nameof(AddToDataReceivedEvent), "called.");

            if (!_OnDataReceived.GetInvocationList().Contains(onDataReceived))
            {
                Logger.LogInfo(nameof(Connection), nameof(AddToDataReceivedEvent), "invocationlist did not contain " + nameof(onDataReceived));

                _OnDataReceived += onDataReceived;
            }
        }

        public void RemoveFromDataReceivedEvent(Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(Connection), nameof(RemoveFromDataReceivedEvent), "called.");

            if (_OnDataReceived.GetInvocationList().Contains(onDataReceived))
            {
                Logger.LogInfo(nameof(Connection), nameof(RemoveFromDataReceivedEvent), "invocationlist did contain " + nameof(onDataReceived));

                _OnDataReceived -= onDataReceived;
            }
        }

        private void OpenConnection()
        {
            Logger.LogInfo(nameof(Connection), nameof(OpenConnection), "called.");

            ParcelFileDescriptor fileDescriptor = _Manager.OpenAccessory(_Accessory);

            Logger.LogInfo(nameof(Connection), nameof(OpenConnection), "accessory opened." + nameof(fileDescriptor) + " created.");

            if (fileDescriptor != null)
            {
                Logger.LogInfo(nameof(Connection), nameof(OpenConnection), nameof(fileDescriptor) + " was not null.");

                _InputStream = new FileInputStream(fileDescriptor.FileDescriptor);
                _OutputStream = new FileOutputStream(fileDescriptor.FileDescriptor);

                Logger.LogInfo(nameof(Connection), nameof(OpenConnection), "Streams retrieved.");
            }
        }

        public void CloseConnection()
        {
            Logger.LogInfo(nameof(Connection), nameof(CloseConnection), "called.");

            _InputStream = null;
            _OutputStream = null;

            _Accessory = null;
            _Manager = null;

            _OnDataReceived = null;

            _TaskCancelToken.Cancel();
        }

        public void SendData(byte[] data)
        {
            Logger.LogInfo(nameof(Connection), nameof(SendData), "called."); 

            if (_OutputStream != null)
            {
                Logger.LogInfo(nameof(Connection), nameof(SendData), nameof(_OutputStream) + " was not null.");

                try
                {
                    _OutputStream.Write(data);

                    Logger.LogInfo(nameof(Connection), nameof(SendData), data.Length + " bytes sent. Message: " + BitConverter.ToString(data));
                }
                catch (Java.Lang.Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void ReceiveData()
        {
            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), "called.");

            while (true)
            {
                if (_InputStream != null)
                {
                    Logger.LogInfo(nameof(Connection), nameof(ReceiveData), nameof(_InputStream) + " was not null.");

                    if (_InputStream.FD != null)
                    {
                        Logger.LogInfo(nameof(Connection), nameof(ReceiveData), nameof(_InputStream.FD) + " was not null.");

                        try
                        {
                            var header = new byte[HEADER_LENGTH];

                            _InputStream.Read(header, 0, header.Length);

                            int bytesRead = 0;
                            int bytesToReceive = Convert.ToInt32(Encoding.ASCII.GetString(header));

                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), "Header with: " + header.Length + " bytes received. Message: " + bytesToReceive.ToString() + " bytes will be received.");

                            var data = new byte[bytesToReceive];

                            do
                            {
                                var tmp =  Convert.ToByte(_InputStream.Read());

                                data[bytesRead] = tmp;

                                bytesRead++;

                            } while (bytesRead != bytesToReceive);

                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), data.Length + " bytes received. Message: " + BitConverter.ToString(data));

                            _OnDataReceived(data);

                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), nameof(_OnDataReceived) + " called.");
                        }
                        catch (Java.Lang.Exception ex)
                        {
                            Logger.LogError(ex);
                        }
                    }                    
                }
                else if (_TaskCancelToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}