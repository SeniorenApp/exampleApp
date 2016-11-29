using Android.App;
using Android.Hardware.Usb;
using Android.Views;
using SeniorenApp.USBCommunication;
using System;
using System.Text;

namespace SeniorenApp.Helper
{
    internal static class USBHelper
    {
        private static Connection _USBConnection;
        private static DisconnectReceiver _DisconnectReceiver;

        public static Connection USBConnection
        {
            get
            {
                Logger.LogInfo(nameof(USBHelper), nameof(USBConnection), "get called.");

                if (_USBConnection == null)
                {                    
                    Logger.LogInfo(nameof(USBHelper), nameof(USBConnection), "was null. Running Non-USB-Mode.");

                    return null;
                }

                return _USBConnection;
            }     
        }

        public static void CreateUSBConnection(Activity activity, Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(USBHelper), nameof(CreateUSBConnection), "called.");

            if (_USBConnection == null)
            {
                UsbAccessory accessory = (UsbAccessory)activity.Intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                UsbManager manager = (UsbManager)activity.GetSystemService("usb");

                Logger.LogInfo(nameof(USBHelper), nameof(CreateUSBConnection), nameof(_USBConnection) + " : " + "was null.");

                _USBConnection = new Connection(accessory, manager, onDataReceived);

                _DisconnectReceiver = new DisconnectReceiver();
            }
        }

        public static void CloseUSBConnection()
        {
            Logger.LogInfo(nameof(USBHelper), nameof(CloseUSBConnection), "called.");

            if (_USBConnection != null)
            {
                _USBConnection.CloseConnection();
            }
        }

        public static void InterpretUSBData(byte[] data, Activity currentActivity, Action<FocusSearchDirection> actionToRunInUIThread)
        {
            Logger.LogInfo(nameof(USBHelper), nameof(InterpretUSBData), "called.");

            string receivedText = Encoding.ASCII.GetString(data);

            Logger.LogInfo(nameof(USBHelper), nameof(InterpretUSBData), "Message decoded to: " + receivedText);

            switch (receivedText)
            {
                case "up":
                    currentActivity.RunOnUiThread(() => actionToRunInUIThread(FocusSearchDirection.Up));
                    break;
                case "down":
                    currentActivity.RunOnUiThread(() => actionToRunInUIThread(FocusSearchDirection.Down)); 
                    break;
                case "left":
                    currentActivity.RunOnUiThread(() => actionToRunInUIThread(FocusSearchDirection.Left));
                    break;
                case "right":
                    currentActivity.RunOnUiThread(() => actionToRunInUIThread(FocusSearchDirection.Right));
                    break;
                case "ok":
                    currentActivity.RunOnUiThread(() => actionToRunInUIThread(FocusSearchDirection.Forward));
                    break;
                default:
                    Logger.LogInfo(nameof(USBHelper), nameof(InterpretUSBData), "Received text could not be interpreted in switch case.");
                    break;
            }
        }
    }
}