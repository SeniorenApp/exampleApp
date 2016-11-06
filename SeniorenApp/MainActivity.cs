using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using Android.Hardware.Usb;
using Android.Content;
using System;
using Java.IO;
using Android.Net;
using Java.Lang;

namespace SeniorenApp
{
    
    [Activity(Label = "SeniorenApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 0;
        UsbAccessory mAccessory;
        ParcelFileDescriptor mFileDescriptor;
        FileInputStream mInputStream;
        FileOutputStream mOutputStream;
        UsbManager mUsbManager;

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
            mUsbManager = (UsbManager)GetSystemService(UsbService);

            IntentFilter i = new IntentFilter();
            i.AddAction(UsbManager.ActionUsbAccessoryAttached);
            i.AddAction(UsbManager.ActionUsbAccessoryDetached);
            i.AddAction("USBPERMISSION");

            RegisterReceiver(new BR(), i);
        }



        public class BR : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                Android.Resource.String action = (Android.Resource.String) intent.Action;
                if (UsbManager.ActionUsbAccessoryAttached.Equals(action))
                {
                    UsbAccessory accessory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    if (intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false))
                    {
                        if(accessory != null)
                        {
                            //call method to set up accessory communication
                            //openAccessory(accessory);
                        }
                    }                    
                }
                else if (UsbManager.ActionUsbAccessoryDetached.Equals(action))
                {
                    UsbAccessory accessory = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    if (accessory != null)
                    {
                        //call method to clean up accessory communication
                        //closeAccessory(accessory);
                    }
                }                
            }
        }
        private void openAccessory()
        {
            mFileDescriptor = mUsbManager.OpenAccessory(mAccessory);
            if(mFileDescriptor != null)
            {
                ParcelFileDescriptor fd = mFileDescriptor;
                //mInputStream = new FileInputStream(fd);
                //mOutputStream = new FileOutputStream(fd);
                Thread thread = new Thread(null, openAccessory, "AccessoryThread");
                thread.Start();
            }
        }

    }
}

    



