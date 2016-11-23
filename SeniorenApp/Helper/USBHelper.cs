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

        public static Connection USBConnection
        {
            get
            {
                if (_USBConnection == null)
                {
                    var ex = new NullReferenceException("USBConnection was not initialized. Call 'CreateUSBConnection' first!");

                    Logger.LogError(ex);

                    throw ex;
                }

                return _USBConnection;
            }     
        }

        public static void CreateUSBConnection(UsbAccessory accessory, UsbManager manager, Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(USBHelper) + " - " + nameof(CreateUSBConnection), "called.");

            if (_USBConnection == null)
            {
                Logger.LogInfo(nameof(USBHelper) + " - " + nameof(CreateUSBConnection), nameof(_USBConnection) + " : " + "was null.");

                _USBConnection = new Connection(accessory, manager, onDataReceived);
            }
        }

        public static void InterpretUSBData(byte[] data, Activity currentActivity, Action<FocusSearchDirection> actionToRunInUIThread)
        {
            Logger.LogInfo(nameof(USBHelper) + " - " + nameof(InterpretUSBData), "called.");

            string receivedText = Encoding.ASCII.GetString(data);

            Logger.LogInfo(nameof(USBHelper) + " - " + nameof(InterpretUSBData), "Message decoded to: " + receivedText);

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
                    Logger.LogInfo(nameof(USBHelper) + " - " + nameof(InterpretUSBData), "Received text could not be interpreted in switch case. Interpreting as 'OK'.");
                    currentActivity.RunOnUiThread(() => actionToRunInUIThread(FocusSearchDirection.Forward));
                    break;
            }
        }
    }
}