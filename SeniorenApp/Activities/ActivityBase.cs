using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Views;
using SeniorenApp.Helper;
using System;

namespace SeniorenApp.Activities
{
    public abstract class ActivityBase : Activity
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
            Logger.LogInfo(GetType().Name, nameof(OnUsbDataReceived), "called.");

            if (IsActive)
            {
                Logger.LogInfo(GetType().Name, nameof(OnUsbDataReceived), "activity is active.");

                try
                {
                    Logger.LogInfo(GetType().Name, nameof(OnUsbDataReceived), data.Length + " bytes received. Message: " + BitConverter.ToString(data));

                    USB.InterpretUSBData(data, this, _HandleUSBData);
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

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Logger.LogInfo(GetType().Name, nameof(OnStart), "Accessory attached.");
                    USB.CreateUSBConnection(this, OnUsbDataReceived);
                    break;
            }

            if (USB.Instance != null)
            {
                USB.Instance.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnNewIntent(Intent intent)
        {
            Logger.LogInfo(GetType().Name, nameof(OnNewIntent), " called");
            Logger.LogInfo(GetType().Name, nameof(OnNewIntent), " Intent is: " + intent.Action.ToString());

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Logger.LogInfo(GetType().Name, nameof(OnStart), "Accessory attached.");
                    USB.CreateUSBConnection(this, OnUsbDataReceived);
                    break;
            }

            base.OnNewIntent(intent);
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

            if (USB.Instance != null)
            {
                USB.Instance.AddToDataReceivedEvent(OnUsbDataReceived);
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

            if (USB.Instance != null)
            {
                USB.Instance.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnDestroy()
        {
            Logger.LogInfo(GetType().Name, nameof(OnDestroy), "called.");

            base.OnDestroy();

            IsActive = false;
        }

        protected void EnableFocusable(View element)
        {
            element.Focusable = true;
            element.FocusableInTouchMode = true;            
        }
        
        protected int NextItemToFocus(View element, FocusSearchDirection direction)
        {
            switch (direction)
            {
                case FocusSearchDirection.Up:
                    return element.NextFocusUpId;
                case FocusSearchDirection.Down:
                    return element.NextFocusDownId;
                case FocusSearchDirection.Left:
                    return element.NextFocusLeftId;
                case FocusSearchDirection.Right:
                    return element.NextFocusRightId;
                default:
                    return element.Id;
            }
        }    
    }
}