using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using Android.Widget;
using SeniorenApp.Helper;
using System.Text;

namespace SeniorenApp
{

    [Activity(Label = "Accessory", MainLauncher = true, Icon = "@drawable/icon")]
    [UsesLibrary("android.hardware.usb.host", Required = true)]
    [IntentFilter(new string[] { "android.hardware.usb.action.USB_ACCESSORY_ATTACHED", "android.hardware.usb.action.USB_ACCESSORY_DETACHED" })]
    [MetaData("android.hardware.usb.action.USB_ACCESSORY_ATTACHED", Resource = "@xml/accessory_filter")]
    [MetaData("android.hardware.usb.action.USB_ACCESSORY_DETACHED", Resource = "@xml/accessory_filter")]    
    public class MainActivity : Activity
    {
        TextView textView;

        static bool isActive = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.Text = "Hallo";

            Log.Info("Accessory", "textbox initialized");

            var test = FindViewById<Button>(Resource.Id.button2);
            test.Click += (sender, e) => { StartActivity(typeof(ManualPhoneCall)); };

            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Click += (sender, e) => USBHelper.USBConnection.SendData(Encoding.ASCII.GetBytes("HALLO"));

            isActive = true;

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Log.Info("Accessory", "accessory attached");
                    UsbAccessory accessory = (UsbAccessory)Intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    UsbManager manager = (UsbManager)GetSystemService(UsbService);
                    USBHelper.CreateUSBConnection(accessory, manager, OnUsbDataReceived);
                    USBHelper.USBConnection.SendData(Encoding.ASCII.GetBytes("hallo"));                    
                    break;
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

        public void OnUsbDataReceived(byte[] data)
        {
            if (isActive)
            {
                try
                {
                    Log.Info("Accessory", nameof(OnUsbDataReceived) + " : " + "called.");

                    string text = Encoding.ASCII.GetString(data);

                    Log.Info("Accessory", nameof(OnUsbDataReceived) + " : " + data.Length + " bytes read. Tanslated to: " + text);

                    RunOnUiThread(() => textView.Text = text);
                }
                catch (Java.Lang.Exception ex)
                {
                    Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
                }
            }            
        }
    }
}





