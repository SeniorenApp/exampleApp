using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using Android.Widget;
using SeniorenApp.USBCommunication;
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
        Connection UsbConnection;

        TextView textView;

        Button sendData;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.Text = "Hallo";

            sendData = FindViewById<Button>(Resource.Id.button1);
            sendData.Click += (sender, e) => UsbConnection.SendData(Encoding.ASCII.GetBytes(textView.Text));

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    UsbAccessory accessory = (UsbAccessory)Intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    UsbManager manager = (UsbManager)GetSystemService(UsbService);
                    UsbConnection = new Connection(accessory, manager, OnUsbDataReceived);
                    UsbConnection.OpenConnection();
                    UsbConnection.SendData(Encoding.ASCII.GetBytes("hallo"));
                    break;
            }            
        }

        public void OnUsbDataReceived(byte[] data)
        {
            try
            {
                Log.Info("Accessory", nameof(OnUsbDataReceived) + " : " + "called.");

                string text = Encoding.ASCII.GetString(data);

                Log.Info("Accessory", nameof(OnUsbDataReceived) + " : " + data.Length + " bytes read. Tanslated to: " + text);

                textView.Text = text;
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Error("Accessory", ex.GetType().Name + System.Environment.NewLine + ex.ToString() + System.Environment.NewLine + ex.StackTrace);
            }
        }
    }
}





