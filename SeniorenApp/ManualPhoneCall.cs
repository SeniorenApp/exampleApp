using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.Lang;
using SeniorenApp.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeniorenApp
{
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon")]
    public class ManualPhoneCall : Activity
    {
        TextView phoneNumber;

        List<Button> Buttons;

        Button button0;
        Button button1;
        Button button2;
        Button button3;
        Button button4;
        Button button5;
        Button button6;
        Button button7;
        Button button8;
        Button button9;
        Button buttonClear;
        Button buttonRemove;
        Button buttonCall;

        static bool isActive = false;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);

                SetContentView(Resource.Layout.ManualCall);

                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);

                Buttons = new List<Button>();

                phoneNumber = FindViewById<TextView>(Resource.Id.PhoneNumber);
                button0 = FindViewById<Button>(Resource.Id.ButtonZero);
                button1 = FindViewById<Button>(Resource.Id.ButtonOne);
                button2 = FindViewById<Button>(Resource.Id.ButtonTwo);
                button3 = FindViewById<Button>(Resource.Id.ButtonThree);
                button4 = FindViewById<Button>(Resource.Id.ButtonFour);
                button5 = FindViewById<Button>(Resource.Id.ButtonFive);
                button6 = FindViewById<Button>(Resource.Id.ButtonSix);
                button7 = FindViewById<Button>(Resource.Id.ButtonSeven);
                button8 = FindViewById<Button>(Resource.Id.ButtonEight);
                button9 = FindViewById<Button>(Resource.Id.ButtonNine);
                buttonClear = FindViewById<Button>(Resource.Id.ButtonClear);
                buttonRemove = FindViewById<Button>(Resource.Id.ButtonRemove);
                buttonCall = FindViewById<Button>(Resource.Id.MakeCall);

                Buttons.Add(button0);
                Buttons.Add(button1);
                Buttons.Add(button2);
                Buttons.Add(button3);
                Buttons.Add(button4);
                Buttons.Add(button5);
                Buttons.Add(button6);
                Buttons.Add(button7);
                Buttons.Add(button8);
                Buttons.Add(button9);
                Buttons.Add(buttonClear);
                Buttons.Add(buttonRemove);
                Buttons.Add(buttonCall);

                isActive = true;

                Log.Info("Accessory", nameof(ManualPhoneCall) + " : " + "called.");
            }
            catch (Exception ex)
            {
                Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
            }            
        }

        public void onStart()
        {
            base.OnStart();

            isActive = true;
        }

        public void onStop()
        {
            base.OnStop();

            isActive = false;
        }

        public void onRestart()
        {
            base.OnRestart();

            isActive = true;
        }

        public void onDestroy()
        {
            base.OnDestroy();

            isActive = false;

            USBHelper.USBConnection.RemoveFromDataReceivedEvent(OnUsbDataReceived);
        }

        [Export("EnterChar")]
        public void EnterChar(View view)
        {
            string character = view.Tag.ToString();

            Log.Info("Accessory", nameof(EnterChar) + " : " + "called.");

            Log.Info("Accessory", nameof(EnterChar) + " : " + character + " entered.");

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

        [Export("Call")]
        public void Call(View view)
        {
            try
            {
                Log.Info("Accessory", nameof(Call) + " : " + "called.");

                var call = new Intent(Intent.ActionCall);

                call.SetData(Uri.Parse("tel:" + phoneNumber.Text));

                StartActivity(call);
            }
            catch (Exception ex)
            {
                Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
            }            
        }

        public void OnUsbDataReceived(byte[] data)
        {
            if (isActive)
            {
                try
                {
                    Log.Info("Accessory", nameof(OnUsbDataReceived) + " : " + "called.");

                    string text = Encoding.ASCII.GetString(data);

                    Log.Info("Accessory", nameof(OnUsbDataReceived) + " : " + data.Length + " bytes read. Tanslated to: " + text);

                    switch (text)
                    {
                        case "up":
                            RunOnUiThread(() => HandleUsbData(FocusSearchDirection.Up));
                            break;
                        case "down":
                            RunOnUiThread(() => HandleUsbData(FocusSearchDirection.Down));
                            break;
                        case "left":
                            RunOnUiThread(() => HandleUsbData(FocusSearchDirection.Left));
                            break;
                        case "right":
                            RunOnUiThread(() => HandleUsbData(FocusSearchDirection.Right));
                            break;
                        default:
                            RunOnUiThread(() => HandleUsbData(FocusSearchDirection.Forward));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private void HandleUsbData(FocusSearchDirection direction)
        {
            var currentlyFocusedButton = Buttons.Where(x => x.IsFocused).FirstOrDefault();

            if (currentlyFocusedButton == null)
            {
                button1.RequestFocus();
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
    }
}