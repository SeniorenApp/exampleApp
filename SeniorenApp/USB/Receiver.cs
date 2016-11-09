using Android.Content;
using Android.Hardware.Usb;
using Java.IO;
using System;
using System.Threading.Tasks;

namespace SeniorenApp.USB
{
    public class Receiver : BroadcastReceiver
    {
        private UsbManager _USBManager;
        private FileInputStream _InputStream;
        private FileOutputStream _OutputStream;

        private Action<FileInputStream> _OnAccessoryOpened;

        public Receiver(UsbManager USBmanager, Action<FileInputStream> onAccessoryOpened)
        {
            _USBManager = USBmanager;

            _OnAccessoryOpened = onAccessoryOpened;
        }

        public override void OnReceive(Context context, Intent intent)
        {           
            var action = (Android.Resource.String)intent.Action;

            if (UsbManager.ActionUsbAccessoryAttached.Equals(action))
            {
                var accessory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);

                if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                {
                    if (accessory != null)
                    {
                        OpenAccessory(accessory);
                    }
                }
            }
            else if (UsbManager.ActionUsbAccessoryDetached.Equals(action))
            {
                var accessory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);

                if (accessory != null)
                {
                    CloseAccessory(accessory);
                }
            }
        }

        private void OpenAccessory(UsbAccessory accessory)
        {
            var fileDescriptor = _USBManager.OpenAccessory(accessory);

            if (fileDescriptor != null)
            {
                _InputStream = new FileInputStream(fileDescriptor.FileDescriptor);
                _OutputStream = new FileOutputStream(fileDescriptor.FileDescriptor);

                Task.Factory.StartNew(() => _OnAccessoryOpened(_InputStream));
            }            
        }

        private void CloseAccessory(UsbAccessory accessory)
        {
            _InputStream = null;
            _OutputStream = null;
        }
    }
}