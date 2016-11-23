using Android.App;
using Android.Content;
using Android.Net;
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
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon")]
    public class ManualPhoneCall : Activity
    {
        private static bool _IsActive = false;

        private static bool IsActive
        {
            get
            {
                Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(IsActive), "Get called. Value was: " + _IsActive.ToString());

                return _IsActive;
            }
            set
            {
                _IsActive = value;

                Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(IsActive), "Set called. Value is now: " + _IsActive.ToString());
            }
        }
            
        private TextView phoneNumber;

        private List<Button> Buttons;

        protected override void OnCreate(Bundle bundle)
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnCreate), " called");
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {               
                base.OnCreate(bundle);

                SetContentView(Resource.Layout.ManualCall);

                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);

                Buttons = new List<Button>()
                {
                    { FindViewById<Button>(Resource.Id.ButtonOne) },
                    { FindViewById<Button>(Resource.Id.ButtonTwo) },
                    { FindViewById<Button>(Resource.Id.ButtonThree) },
                    { FindViewById<Button>(Resource.Id.ButtonFour) },
                    { FindViewById<Button>(Resource.Id.ButtonFive) },
                    { FindViewById<Button>(Resource.Id.ButtonSix) },
                    { FindViewById<Button>(Resource.Id.ButtonSeven) },
                    { FindViewById<Button>(Resource.Id.ButtonEight) },
                    { FindViewById<Button>(Resource.Id.ButtonNine) },
                    { FindViewById<Button>(Resource.Id.ButtonZero) },
                    { FindViewById<Button>(Resource.Id.ButtonClear) },
                    { FindViewById<Button>(Resource.Id.ButtonRemove) },
                    { FindViewById<Button>(Resource.Id.MakeCall) },
                };

                phoneNumber = FindViewById<TextView>(Resource.Id.PhoneNumber);

                IsActive = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        protected override void OnStart()
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnStart), "called.");

            base.OnStart();

            IsActive = true;
        }

        protected override void OnStop()
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnStop), "called.");

            base.OnStop();

            IsActive = false;
        }

        protected override void OnRestart()
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnRestart), "called.");

            base.OnRestart();

            IsActive = true;
        }

        protected override void OnDestroy()
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnDestroy), "called.");

            base.OnDestroy();

            IsActive = false;

            USBHelper.USBConnection.RemoveFromDataReceivedEvent(OnUsbDataReceived);
        }

        [Export("EnterChar")]
        public void EnterChar(View view)
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(EnterChar), "called.");

            try
            {                
                string character = view.Tag.ToString();

                Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(EnterChar), "Entered character was: " + character);

                switch (character)
                {
                    case "<":
                        phoneNumber.Text = phoneNumber.Text == string.Empty ? string.Empty : phoneNumber.Text.Remove(phoneNumber.Text.Length - 1, 1);
                        break;
                    case "CLR":
                        phoneNumber.Text = string.Empty;
                        break;
                    default:
                        phoneNumber.Text += character;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        [Export("Call")]
        public void Call(View view)
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(Call), "called.");

            try
            {                
                var call = new Intent(Intent.ActionCall);

                call.SetData(Uri.Parse("tel:" + phoneNumber.Text));

                StartActivity(call);

                Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(Call), "Activity: " + nameof(call) + " started.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        public void OnUsbDataReceived(byte[] data)
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnUsbDataReceived), "called.");

            if (IsActive)
            {
                Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(OnUsbDataReceived), "activity is active.");

                try
                {
                    Logger.LogInfo(nameof(MainActivity) + " - " + nameof(OnUsbDataReceived), data.Length + " bytes received. Message: " + System.BitConverter.ToString(data));

                    USBHelper.InterpretUSBData(data, this, HandleUsbData);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void HandleUsbData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(HandleUsbData), "called.");
            Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(HandleUsbData), nameof(FocusSearchDirection) + " is: " + direction.ToString());

            try
            {
                var currentlyFocusedButton = Buttons.Where(x => x.IsFocused).FirstOrDefault();

                if (currentlyFocusedButton == null)
                {
                    Logger.LogInfo(nameof(ManualPhoneCall) + " - " + nameof(HandleUsbData), nameof(currentlyFocusedButton) + " was null.");

                    Buttons.First(x => x.Tag.ToString() == "1").RequestFocus();
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