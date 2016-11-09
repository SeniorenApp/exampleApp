using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Widget;
using Java.IO;
using SeniorenApp.USB;

namespace SeniorenApp
{

    [Activity(Label = "SeniorenApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var add = FindViewById<Button>(Resource.Id.button1);
            var sub = FindViewById<Button>(Resource.Id.button2);
            var tex = FindViewById<TextView>(Resource.Id.textView1);

            add.Click += (sender, e) => { count++; tex.Text = count.ToString(); };
            sub.Click += (sender, e) => { count--; tex.Text = count.ToString(); };

            var i = new IntentFilter();
            i.AddAction(UsbManager.ActionUsbAccessoryAttached);
            i.AddAction(UsbManager.ActionUsbAccessoryDetached);
            i.AddAction("USBPERMISSION");

            RegisterReceiver(new Receiver((UsbManager)GetSystemService(UsbService), OnAccessoryOpened), i);
        }

        public void OnAccessoryOpened(FileInputStream stream)
        {
            var tex = FindViewById<TextView>(Resource.Id.textView1);

            tex.Text = "Accessory opened";
        }
    }
}

    



