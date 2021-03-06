﻿using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;
using SeniorenApp.Data;
using SeniorenApp.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SeniorenApp.Activities
{
    /// <summary>
    /// Main activity which starts all other activities.
    /// Is also automatically launched if an an accessory which matches the accessory filter has been attached.
    /// For detecting an new intent while this activity is active (USB device plugged in) LaunchMode has been set to SingleTop.
    /// </summary>
    [Activity(Label = Constants.MainActivityLabel, MainLauncher = true, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    [UsesLibrary(Constants.AndroidUSBHostLibrary, Required = true)]
    [IntentFilter(new string[] { UsbManager.ActionUsbAccessoryAttached, UsbManager.ActionUsbAccessoryDetached })]
    [MetaData(UsbManager.ActionUsbAccessoryAttached, Resource = Constants.AccessoryFilterLocation)]
    [MetaData(UsbManager.ActionUsbAccessoryDetached, Resource = Constants.AccessoryFilterLocation)]    
    public class MainActivity : ActivityBase
    {
        private List<Button> _Buttons;

        public MainActivity()
        {
            _HandleUSBData = HandleUSBData;
            _OnConnectionClosed = OnUSBConnectionClosed;
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
                    { FindViewById<Button>(Resource.Id.CameraActivity) },
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
            base.OnStart();

            _Buttons.ForEach(x => EnableFocusable(x));
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            _Buttons.ForEach(x => EnableFocusable(x));
        }

        [Export(nameof(StartPhoneCallActivity))]
        public void StartPhoneCallActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartPhoneCallActivity), "called.");

            StartActivity(typeof(ManualPhoneCall));
        }

        [Export(nameof(StartContactListActivity))]
        public void StartContactListActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartContactListActivity), "called.");

            StartActivity(typeof(ContactList));
        }

        [Export(nameof(StartCameraActivity))]
        public void StartCameraActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartCameraActivity), "called.");

            StartActivity(typeof(CameraToRead));
        }

        [Export(nameof(StartAboutActivity))]
        public void StartAboutActivity(View view)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(StartAboutActivity), "called.");

            StartActivity(typeof(About));
        }

        private void OnUSBConnectionClosed()
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnUSBConnectionClosed), "called.");

            _Buttons.ForEach(x => DisableFocusable(x));
        }

        private void HandleUSBData(USBCommand command)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), nameof(command) + " is: " + command.ToString());

            try
            {
                var currentlyFocusedButton = _Buttons.Where(x => x.IsFocused).FirstOrDefault();

                if (currentlyFocusedButton == null)
                {
                    Logger.LogInfo(nameof(MainActivity), nameof(HandleUSBData), nameof(currentlyFocusedButton) + " was null.");

                    _Buttons.First().RequestFocus();
                }
                else if (command == USBCommand.ok)
                {
                    currentlyFocusedButton.CallOnClick();
                }
                else
                {
                    FindViewById<Button>(NextItemToFocus(currentlyFocusedButton, command)).RequestFocus();
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}





