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
    /// <summary>
    /// Actual implementation of the usb connection.
    /// </summary>
    internal class Connection
    {        
        private UsbAccessory _Accessory;
        private UsbManager _Manager;

        // Streams and descriptors for reading and writing. Inputstream for reading. Outputstream for sending.
        private ParcelFileDescriptor _FileDescriptor; 
        private FileInputStream _InputStream;
        private FileOutputStream _OutputStream;
                
        // Cancellationtoken for the receivedata task which runs in a different thread.
        private CancellationTokenSource _TaskCancelTokenSource;
        private CancellationToken _TaskCancelToken;

        // OnDataReceived is called once data has been received.
        // Theoretically an unlimited amount of functions can be added the events (ondatareceived and onconnectionclosed).
        // Via AddToDataReceiveEvent and AddToConnectionClosedEvent additional functions can be added.
        // Although they should be removed via RemoveFromDataReceivedEvent and RemoveFromConnectionClosedEvent as memory leaks can occur if you are not careful.
        // Via this events the received data can be interpreted by various activities.
        private Action<byte[]> _OnDataReceived;
        private Action _OnConnectionClosed;

        private bool _IsConnected;

        public bool IsConnected
        {
            get { return _IsConnected; }
        }

        public Connection(UsbAccessory accessory, UsbManager manager, Action<byte[]> onDataReceived, Action onConnectionClosed)
        {
            Logger.LogInfo(nameof(Connection), "Constructor", "called.");

            try
            {
                _Accessory = accessory;
                _Manager = manager;

                _OnDataReceived += onDataReceived;
                _OnConnectionClosed += onConnectionClosed;

                OpenConnection();

                _TaskCancelTokenSource = new CancellationTokenSource();
                _TaskCancelToken = _TaskCancelTokenSource.Token;

                // Start the receivedata task in a new thread, with the given canceltoken.
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

        public void AddToConnectionClosedEvent(Action onConnectionClosed)
        {
            Logger.LogInfo(nameof(Connection), nameof(AddToConnectionClosedEvent), "called.");

            if (!_OnConnectionClosed.GetInvocationList().Contains(onConnectionClosed))
            {
                Logger.LogInfo(nameof(Connection), nameof(AddToConnectionClosedEvent), "invocationlist did not contain " + nameof(onConnectionClosed));

                _OnConnectionClosed += onConnectionClosed;
            }
        }

        public void RemoveFromConnectionClosedEvent(Action onConnectionClosed)
        {
            Logger.LogInfo(nameof(Connection), nameof(RemoveFromConnectionClosedEvent), "called.");

            if (_OnConnectionClosed.GetInvocationList().Contains(onConnectionClosed))
            {
                Logger.LogInfo(nameof(Connection), nameof(RemoveFromConnectionClosedEvent), "invocationlist did contain " + nameof(onConnectionClosed));

                _OnConnectionClosed -= onConnectionClosed;
            }
        }

        private void OpenConnection()
        {
            Logger.LogInfo(nameof(Connection), nameof(OpenConnection), "called.");

            _FileDescriptor = _Manager.OpenAccessory(_Accessory);

            Logger.LogInfo(nameof(Connection), nameof(OpenConnection), "accessory opened." + nameof(_FileDescriptor) + " created.");

            if (_FileDescriptor != null)
            {
                Logger.LogInfo(nameof(Connection), nameof(OpenConnection), nameof(_FileDescriptor) + " was not null.");

                _InputStream = new FileInputStream(_FileDescriptor.FileDescriptor);
                _OutputStream = new FileOutputStream(_FileDescriptor.FileDescriptor);

                Logger.LogInfo(nameof(Connection), nameof(OpenConnection), "Streams retrieved.");

                _IsConnected = true;
            }
        }

        public void CloseConnection()
        {
            Logger.LogInfo(nameof(Connection), nameof(CloseConnection), "called.");

            try
            {
                // Cancel the receive data task and dispose the rest.
                _TaskCancelTokenSource.Cancel();

                _IsConnected = false;

                _OnConnectionClosed();

                _InputStream.Dispose();
                _OutputStream.Dispose();

                _Accessory.Dispose();
                _Manager.Dispose();

                _OnDataReceived = null;
                _OnConnectionClosed = null;                
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
                        // If the connection has been closed. Cancel the task.
                        if (_TaskCancelToken.IsCancellationRequested)
                        {
                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), "cancelled.");
                            return;
                        }

                        var data = new List<byte>();

                        // The do-while loop waits for data. InputStream.Read blocks the current thread until data has been received.
                        // Once data has been received the loop will read until the endofstreambyte (0x04) has been detected.
                        do
                        {
                            var buffer = new byte[Constants.USBMaxPacketSize];

                            _InputStream.Read(buffer);

                            data.AddRange(buffer);

                            Logger.LogInfo(nameof(Connection), nameof(ReceiveData), "Complete message: " + BitConverter.ToString(data.ToArray()));

                        } while (!data.Any(x => x == Constants.EndOfStreamByte));

                        Logger.LogInfo(nameof(Connection), nameof(ReceiveData), data.Count + " bytes received. Message: " + BitConverter.ToString(data.ToArray()));

                        // Cut off the garbage data that comes after the endofstreambyte.
                        data.RemoveRange(data.IndexOf(Constants.EndOfStreamByte), data.Count - data.IndexOf(Constants.EndOfStreamByte));
                        
                        // And call the functions which subscribe to the ondatareceived event.
                        _OnDataReceived(data.ToArray());

                        Logger.LogInfo(nameof(Connection), nameof(ReceiveData), nameof(_OnDataReceived) + " called.");
                    }
                    catch (Java.Lang.Exception ex)
                    {                        
                        Logger.LogError(ex);
                        return;
                    }
                }
            }
        }
    }
}