using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Views;
using SeniorenApp.Helper;
using System;

namespace SeniorenApp
{
    public class ActivityBase : Activity
    {
        protected bool _IsActive = false;
        protected Action<FocusSearchDirection> _HandleUSBData;

        protected bool IsActive
        {
            get
            {
                Logger.LogInfo(GetType().Name, nameof(IsActive), "Get called. Value was: " + _IsActive.ToString());

                return _IsActive;
            }
            set
            {
                _IsActive = value;

                Logger.LogInfo(GetType().Name, nameof(IsActive), "Set called. Value is now: " + _IsActive.ToString());
            }
        }

        protected void OnUsbDataReceived(byte[] data)
        {
            Logger.LogInfo(nameof(MainActivity), nameof(OnUsbDataReceived), "called.");

            if (IsActive)
            {
                Logger.LogInfo(nameof(MainActivity), nameof(OnUsbDataReceived), "activity is active.");

                try
                {
                    Logger.LogInfo(nameof(MainActivity), nameof(OnUsbDataReceived), data.Length + " bytes received. Message: " + System.BitConverter.ToString(data));

                    USBHelper.InterpretUSBData(data, this, _HandleUSBData);
                }
                catch (Java.Lang.Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        protected override void OnStart()
        {
            Logger.LogInfo(GetType().Name, nameof(OnStart), "called.");

            base.OnStart();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnStop()
        {
            Logger.LogInfo(GetType().Name, nameof(OnStop), "called.");

            base.OnStop();

            IsActive = false;
        }

        protected override void OnRestart()
        {
            Logger.LogInfo(GetType().Name, nameof(OnRestart), "called.");

            base.OnRestart();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnPause()
        {
            Logger.LogInfo(GetType().Name, nameof(OnPause), "called.");

            base.OnPause();

            IsActive = false;
        }

        protected override void OnResume()
        {
            Logger.LogInfo(GetType().Name, nameof(OnResume), "called.");

            base.OnResume();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnDestroy()
        {
            Logger.LogInfo(GetType().Name, nameof(OnDestroy), "called.");

            base.OnDestroy();

            IsActive = false;
        }
    }
}