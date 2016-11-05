using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using Android.Hardware.Usb;
using Android.Content;
using System;

namespace SeniorenApp
{
    
    [Activity(Label = "SeniorenApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 0;
        public static UsbAccessory mAccessory = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button add = FindViewById<Button>(Resource.Id.button1);
            Button sub = FindViewById<Button>(Resource.Id.button2);
            var tex = FindViewById<TextView>(Resource.Id.textView1);


            add.Click += delegate
            {
                count++;
                tex.Text = count.ToString();
            };
            sub.Click += delegate
            {
                count--;
                tex.Text = count.ToString();
            };
            //UsbAccessory accessory;// = UsbManager.ActionUsbAccessoryAttached;



            IntentFilter i = new IntentFilter();
            i.AddAction(UsbManager.ActionUsbAccessoryAttached);
            i.AddAction(UsbManager.ActionUsbAccessoryDetached);
            i.AddAction("USBPERMISSION");
            RegisterReceiver(new BR(), i);

            UsbManager manager = (UsbManager)GetSystemService(UsbService);

            var devices = manager.GetAccessoryList();





        }
        public class BR : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                String action = intent.Action;
                if (UsbManager.ActionUsbAccessoryAttached.Equals(action))
                {
                    UsbAccessory accessory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                    {
                        mAccessory = accessory;
                        ParcelFileDescriptor fd = null;

                    }
                }
            }
        }
    }

    

}

