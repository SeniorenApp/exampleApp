using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using SeniorenApp.Data;
using SeniorenApp.Helper;
using System.Collections.Generic;
using System.Linq;

namespace SeniorenApp.Activities
{
    /// <summary>
    /// Call activity for entering the phone number manually.
    /// For detecting an new intent while this activity is active (USB device plugged in) LaunchMode has been set to SingleTop.
    /// </summary>
    [Activity(Label = Constants.ManualPhoneCallActivityLabel, MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ManualPhoneCall : ActivityBase
    {
        private TextView _PhoneNumber;
        private List<Button> _Buttons;

        public ManualPhoneCall()
        {
            _HandleUSBData = HandleUsbData;
            _OnConnectionClosed = OnUSBConnectionClosed;
        }

        protected override void OnCreate(Bundle bundle)
        {
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(OnCreate), " called");
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {               
                base.OnCreate(bundle);

                SetContentView(Resource.Layout.ManualCall);                               

                _Buttons = new List<Button>()
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
                    { FindViewById<Button>(Resource.Id.GoBack) },
                };

                _Buttons.ForEach(x => EnableFocusable(x));

                _PhoneNumber = FindViewById<TextView>(Resource.Id.PhoneNumber);               

                IsActive = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            _Buttons.ForEach(x => EnableFocusable(x));
        }

        [Export(nameof(EnterChar))]
        public void EnterChar(View view)
        {
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(EnterChar), "called.");

            try
            {                
                string character = view.Tag.ToString();

                Logger.LogInfo(nameof(ManualPhoneCall), nameof(EnterChar), "Entered character was: " + character);

                switch (character)
                {
                    case "<":
                        // If the phone number is not empty remove the last character.
                        _PhoneNumber.Text = _PhoneNumber.Text == string.Empty ? string.Empty : _PhoneNumber.Text.Remove(_PhoneNumber.Text.Length - 1, 1);
                        break;
                    case "CLR":
                        _PhoneNumber.Text = string.Empty;
                        break;
                    default:
                        _PhoneNumber.Text += character;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        [Export(nameof(Call))]
        public void Call(View view)
        {
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(Call), "called.");

            try
            {                
                var call = new Intent(Intent.ActionCall);

                call.SetData(Uri.Parse("tel:" + _PhoneNumber.Text));

                StartActivity(call);

                Logger.LogInfo(nameof(ManualPhoneCall), nameof(Call), "Activity: " + nameof(call) + " started.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        [Export(nameof(GoToPreviousActivity))]
        public void GoToPreviousActivity(View view)
        {
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(GoToPreviousActivity), "called.");

            Finish();
        }

        private void OnUSBConnectionClosed()
        {
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(OnUSBConnectionClosed), "called.");

            _Buttons.ForEach(x => DisableFocusable(x));
        }

        private void HandleUsbData(USBCommand command)
        {
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(HandleUsbData), "called.");
            Logger.LogInfo(nameof(ManualPhoneCall), nameof(HandleUsbData), nameof(command) + " is: " + command.ToString());           

            try
            {
                var currentlyFocusedButton = _Buttons.Where(x => x.IsFocused).FirstOrDefault();

                if (currentlyFocusedButton == null)
                {
                    Logger.LogInfo(nameof(ManualPhoneCall), nameof(HandleUsbData), nameof(currentlyFocusedButton) + " was null.");

                    _Buttons.First(x => x.Tag.ToString() == "1").RequestFocus();
                }
                else if (command == USBCommand.ok)
                {
                    Logger.LogInfo(nameof(ManualPhoneCall), nameof(HandleUsbData), currentlyFocusedButton.Id + " called on click.");

                    currentlyFocusedButton.CallOnClick();
                }
                else
                {
                    FindViewById<Button>(NextItemToFocus(currentlyFocusedButton, command)).RequestFocus();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }            
        }
    }
}