using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using SeniorenApp.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SeniorenApp
{

    [Activity(Label = "Accessory", MainLauncher = true, Icon = "@drawable/icon")]
    [UsesLibrary("android.hardware.usb.host", Required = true)]
    [IntentFilter(new string[] { "android.hardware.usb.action.USB_ACCESSORY_ATTACHED", "android.hardware.usb.action.USB_ACCESSORY_DETACHED" })]
    [MetaData("android.hardware.usb.action.USB_ACCESSORY_ATTACHED", Resource = "@xml/accessory_filter")]
    [MetaData("android.hardware.usb.action.USB_ACCESSORY_DETACHED", Resource = "@xml/accessory_filter")]    
    public class MainActivity : Activity
    {
        private List<ImageButton> _Buttons;
        private static bool _IsActive = false;

        private static bool IsActive
        {
            get
            {
                Logger.LogInfo(nameof(MainActivity), nameof(IsActive), "Get called. Value was: " + _IsActive.ToString());

                return _IsActive;
            }
            set
            {
                _IsActive = value;

                Logger.LogInfo(nameof(MainActivity), nameof(IsActive), "Set called. Value is now: " + _IsActive.ToString());
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnCreate), " called");
            Logger.LogInfo(nameof(MainActivity), nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {                
                base.OnCreate(bundle);

                SetContentView(Resource.Layout.Main);

                _Buttons = new List<ImageButton>
                {
                    { FindViewById<ImageButton>(Resource.Id.CallActivity) },
                    { FindViewById<ImageButton>(Resource.Id.Temp1) },
                    { FindViewById<ImageButton>(Resource.Id.Temp2) },
                    { FindViewById<ImageButton>(Resource.Id.Temp3) },
                };

                IsActive = true;                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }                        
        }

        protected override void OnNewIntent(Intent intent)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnNewIntent), " called");
            Logger.LogInfo(nameof(MainActivity), nameof(OnNewIntent), " Intent is: " + intent.ToString());

            base.OnNewIntent(intent);

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Logger.LogInfo(nameof(MainActivity), nameof(OnNewIntent), "Accessory attached.");
                    USBHelper.CreateUSBConnection(this, OnUsbDataReceived);
                    break;
            }
        }

        protected override void OnStart()
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnStart), "called.");

            base.OnStart();

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Logger.LogInfo(nameof(MainActivity), nameof(OnStart), "Accessory attached.");
                    USBHelper.CreateUSBConnection(this, OnUsbDataReceived);
                    break;
            }

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnStop()
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnStop), "called.");

            base.OnStop();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.RemoveFromDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = false;
        }

        protected override void OnRestart()
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnRestart), "called.");

            base.OnRestart();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnDestroy()
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnDestroy), "called.");

            base.OnDestroy();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.RemoveFromDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = false;
        }

        [Export("StartPhoneCallActivity")]
        public void StartPhoneCallActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartPhoneCallActivity), "called.");

            StartActivity(typeof(ManualPhoneCall));
        }

        [Export("StartContactListActivity")]
        public void StartContactListActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartContactListActivity), "called.");

            StartActivity(typeof(ContactList));
        }

        public void OnUsbDataReceived(byte[] data)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnUsbDataReceived), "called.");

            if (IsActive)
            {
                Logger.LogInfo(nameof(MainActivity), nameof(OnUsbDataReceived), "activity is active.");

                try
                {
                    Logger.LogInfo(nameof(MainActivity), nameof(OnUsbDataReceived), data.Length + " bytes received. Message: " + System.BitConverter.ToString(data));

                    USBHelper.InterpretUSBData(data, this, HandleUSBData);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }            
        }

        private void HandleUSBData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), nameof(FocusSearchDirection) + " is: " + direction.ToString());

            try
            {
                var currentlyFocusedButton = _Buttons.Where(x => x.IsFocused).FirstOrDefault();

                if (currentlyFocusedButton == null)
                {
                    Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), nameof(currentlyFocusedButton) + " was null.");

                    _Buttons.First().RequestFocus();
                }
                else if (direction == FocusSearchDirection.Forward)
                {
                    currentlyFocusedButton.CallOnClick();
                }
                else
                {
                    currentlyFocusedButton.RequestFocus(direction);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}





