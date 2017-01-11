using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using SeniorenApp.Data;
using SeniorenApp.USBCommunication;
using System;
using System.Text;

namespace SeniorenApp.Helper
{
    /// <summary>
    /// Implements the actual usb connection via singleton pattern, whichs means only one connection
    /// can exist in the whole program. 
    /// </summary>
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

        public static bool IsConnected
        {
            get { return Instance == null ? false : Instance.IsConnected; }
        }

        public static void CreateUSBConnection(Activity activity, Action<byte[]> onDataReceived, Action onConnectionClosed, Intent intent = null)
        {
            Logger.LogInfo(nameof(USB), nameof(CreateUSBConnection), "called.");

            if (_Instance == null)
            {
                UsbAccessory accessory = intent == null ? (UsbAccessory)activity.Intent.GetParcelableExtra(UsbManager.ExtraAccessory) : (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                UsbManager manager = (UsbManager)activity.GetSystemService("usb");

                Logger.LogInfo(nameof(USB), nameof(CreateUSBConnection), nameof(_Instance) + " : " + "was null.");

                _Instance = new Connection(accessory, manager, onDataReceived, onConnectionClosed);
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

        public static void InterpretUSBData(byte[] data, Activity currentActivity, Action<USBCommand> actionToRunInUIThread)
        {
            Logger.LogInfo(nameof(USB), nameof(InterpretUSBData), "called.");

            string receivedText = Encoding.ASCII.GetString(data);

            Logger.LogInfo(nameof(USB), nameof(InterpretUSBData), "Message decoded to: " + receivedText);

            USBCommand receivedCommand;

            if (Enum.TryParse<USBCommand>(receivedText, out receivedCommand))
            {
                currentActivity.RunOnUiThread(() => actionToRunInUIThread(receivedCommand));
            }
            else
            {
                Logger.LogInfo(nameof(USB), nameof(InterpretUSBData), "Received text could not be interpreted.");
            }
        }
    }
}