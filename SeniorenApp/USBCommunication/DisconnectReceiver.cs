using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using SeniorenApp.Helper;

namespace SeniorenApp.USBCommunication
{
    [BroadcastReceiver]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/accessory_filter")]
    [MetaData(UsbManager.ActionUsbDeviceDetached, Resource = "@xml/accessory_filter")]
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