using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;
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
    public class MainActivity : ActivityBase
    {
        private List<Button> _Buttons;

        public MainActivity()
        {
            _HandleUSBData = HandleUSBData;
        }

        protected override void OnCreate(Bundle bundle)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnCreate), " called");
            Logger.LogInfo(nameof(MainActivity), nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {                
                base.OnCreate(bundle);

                SetContentView(Resource.Layout.Main);

                _Buttons = new List<Button>
                {
                    { FindViewById<Button>(Resource.Id.CallActivity) },
                    { FindViewById<Button>(Resource.Id.ContactListActivity) },
                    { FindViewById<Button>(Resource.Id.Temp2) },
                    { FindViewById<Button>(Resource.Id.AboutActivity) },
                };

                _Buttons.ForEach(x => EnableFocusable(x));

                IsActive = true;                
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }                        
        }

        protected override void OnStart()
        {
            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Logger.LogInfo(nameof(MainActivity), nameof(OnStart), "Accessory attached.");
                    USBHelper.CreateUSBConnection(this, OnUsbDataReceived);
                    break;
            }

            base.OnStart();           
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

        [Export("StartAboutActivity")]
        public void StartAboutActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartAboutActivity), "called.");

            StartActivity(typeof(About));
        }

        private void HandleUSBData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), "called.");

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
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}





