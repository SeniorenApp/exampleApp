using Android.Content;
using Android.Hardware.Usb;
using SeniorenApp.Helper;

namespace SeniorenApp.USBCommunication
{
    [BroadcastReceiver]
    internal class DisconnectReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Logger.LogInfo(nameof(DisconnectReceiver), nameof(OnReceive), "called.");
            Logger.LogInfo(nameof(DisconnectReceiver), nameof(OnReceive), "Intent was: " + intent.Action);

            if (intent.Action == UsbManager.ActionUsbAccessoryDetached)
            {
                var acceesory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);

                if (acceesory == null)
                {
                    USBHelper.CloseUSBConnection();
                }
            }
        }
    }
}