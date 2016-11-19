using Android.Util;
using Java.IO;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SeniorenApp.USBCommunication
{
    internal class CommunicationHandler
    {
        private FileInputStream _InputStream;
        private FileOutputStream _OutputStream;
        private Action<byte[]> _OnDataReceived;

        public CommunicationHandler(FileInputStream inputStream, FileOutputStream outputStream, Action<byte[]> onDataReceived)
        {
            _InputStream = inputStream;
            _OutputStream = outputStream;
            _OnDataReceived = onDataReceived;

            Log.Info("Accessory", nameof(CommunicationHandler) + " : " + "created.");

            Task.Factory.StartNew(() => ReceiveData());

            Log.Info("Accessory", nameof(CommunicationHandler) + " : " + "task created.");
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
                    Log.Error("Accessory", ex.GetType().Name + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.StackTrace);
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
                        Log.Error("Accessory", ex.GetType().Name + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.StackTrace);
                    }
                }                
            }
        }
    }
}