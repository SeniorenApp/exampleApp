using Android.Hardware.Usb;
using Android.OS;
using Java.IO;
using SeniorenApp.Data;
using SeniorenApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SeniorenApp.USBCommunication
{
    internal class Connection
    {
        private UsbAccessory _Accessory;
        private UsbManager _Manager;
        private FileInputStream _InputStream;
        private FileOutputStream _OutputStream;

        private CancellationTokenSource _TaskCancelTokenSource;
        private CancellationToken _TaskCancelToken;

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

                _TaskCancelTokenSource = new CancellationTokenSource();
                _TaskCancelToken = _TaskCancelTokenSource.Token;

                Task.Factory.StartNew(() => ReceiveData(), _TaskCancelToken);

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

            try
            {
                _InputStream.Dispose();
                _OutputStream.Dispose();

                _Accessory.Dispose();
                _Manager.Dispose();

                _OnDataReceived = null;

                _TaskCancelTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
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

                    try
                    {
                        if (_TaskCancelToken.IsCancellationRequested)
                        {
                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), "cancelled.");
                            return;
                        }

                        var data = new List<byte>();

                        do
                        {
                            var buffer = new byte[Constants.USBMaxPacketSize];

                            _InputStream.Read(buffer);

                            data.AddRange(buffer);

                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), "Complete message: " + BitConverter.ToString(data.ToArray()));

                        } while (!data.Any(x => x == Constants.EndOfStreamByte));

                        Logger.LogInfo(nameof(Connection), nameof(ReceiveData), data.Count + " bytes received. Message: " + BitConverter.ToString(data.ToArray()));

                        data.RemoveRange(data.IndexOf(Constants.EndOfStreamByte), data.Count - data.IndexOf(Constants.EndOfStreamByte));

                        _OnDataReceived(data.ToArray());

                        Logger.LogInfo(nameof(Connection), nameof(ReceiveData), nameof(_OnDataReceived) + " called.");
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