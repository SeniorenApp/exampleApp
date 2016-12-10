using Android.App;
using Android.Hardware.Usb;
using Android.Views;
using SeniorenApp.USBCommunication;
using System;
using System.Text;

namespace SeniorenApp.Helper
{
    internal static class USB
    {
        private static Connection _Instance;

        public static Connection Instance
        {
            get
            {
                Logger.LogInfo(nameof(USB), nameof(Instance), "get called.");

                if (_Instance == null)
                {                    
                    Logger.LogInfo(nameof(USB), nameof(Instance), "was null. Running Non-USB-Mode.");

                    return null;
                }

                return _Instance;
            }     
        }

        public static void CreateUSBConnection(Activity activity, Action<byte[]> onDataReceived)
        {
            Logger.LogInfo(nameof(USB), nameof(CreateUSBConnection), "called.");

            if (_Instance == null)
            {
                UsbAccessory accessory = (UsbAccessory)activity.Intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                UsbManager manager = (UsbManager)activity.GetSystemService("usb");

                Logger.LogInfo(nameof(USB), nameof(CreateUSBConnection), nameof(_Instance) + " : " + "was null.");

                _Instance = new Connection(accessory, manager, onDataReceived);
            }
        }

        public static void CloseUSBConnection()
        {
            Logger.LogInfo(nameof(USB), nameof(CloseUSBConnection), "called.");

            if (_Instance != null)
            {
                _Instance.CloseConnection();
                _Instance = null;
            }
        }

        public static void InterpretUSBData(byte[] data, Activity currentActivity, Action<FocusSearchDirection> actionToRunInUIThread)
        {
            Logger.LogInfo(nameof(USB), nameof(InterpretUSBData), "called.");

            string receivedText = Encoding.ASCII.GetString(data);

            Logger.LogInfo(nameof(USB), nameof(InterpretUSBData), "Message decoded to: " + receivedText);

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
                    Logger.LogInfo(nameof(USB), nameof(InterpretUSBData), "Received text could not be interpreted in switch case.");
                    break;
            }
        }
    }
}