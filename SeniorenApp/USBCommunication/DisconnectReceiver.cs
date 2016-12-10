using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using SeniorenApp.Data;
using SeniorenApp.Helper;

namespace SeniorenApp.USBCommunication
{
    [BroadcastReceiver]
    [IntentFilter(new[] { UsbManager.ActionUsbAccessoryAttached, UsbManager.ActionUsbAccessoryDetached })]
    [MetaData(UsbManager.ActionUsbAccessoryAttached, Resource = Constants.AccessoryFilterLocation)]
    [MetaData(UsbManager.ActionUsbAccessoryDetached, Resource = Constants.AccessoryFilterLocation)]
    internal class DisconnectReceiver : BroadcastReceiver
    {
        public DisconnectReceiver()
        {
            Logger.LogInfo(nameof(DisconnectReceiver), "Constructor", "DisconnectReceiver created.");
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Logger.LogInfo(nameof(DisconnectReceiver), nameof(OnReceive), "called.");
            Logger.LogInfo(nameof(DisconnectReceiver), nameof(OnReceive), "Intent was: " + intent.Action);

            if (intent.Action == UsbManager.ActionUsbAccessoryDetached)
            {
                var acceesory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);

                if (acceesory != null)
                {
                    USB.CloseUSBConnection();
                }
            }
        }
    }
}