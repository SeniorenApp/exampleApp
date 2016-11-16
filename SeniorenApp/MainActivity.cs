using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Widget;
using Java.IO;
using System.Linq;
using System;

namespace SeniorenApp
{

    [Activity(Label = "SeniorenApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.Text = "bla";

            //var i = new IntentFilter();
            //i.AddAction(UsbManager.ActionUsbAccessoryAttached);
            //i.AddAction(UsbManager.ActionUsbAccessoryDetached);
            //i.AddAction("USBPERMISSION");

            if (UsbManager.ActionUsbAccessoryAttached.Equals(Intent.Action))
            {
                UsbManager manager = (UsbManager)GetSystemService(Context.UsbService);

                if (manager != null)
                {
                    UsbAccessory[] accessoryList = manager.GetAccessoryList();

                    if (accessoryList != null)
                    {
                        if (accessoryList.Any())
                        {
                            textView.Text = "accessory attached - accessory found";
                        }
                        else
                        {
                            textView.Text = "accessory attached - No accessory found";
                        }
                    }

                    textView.Text = "accessory attached - accessory list is null";
                }                
            }
            else
            {
                textView.Text = "Accessory not attached - Action was:" + Intent.Action;
            }

            // RegisterReceiver(new Receiver((UsbManager)GetSystemService(UsbService), OnAccessoryOpened), i);
        }

        //protected override void OnNewIntent(Intent intent)
        //{
        //    SetContentView(Resource.Layout.Main);

        //    var textView = FindViewById<TextView>(Resource.Id.textView1);
        //    textView.Text = "bla";

        //    if (UsbManager.ActionUsbAccessoryAttached.Equals(intent.Action))
        //    {
        //        UsbManager manager = (UsbManager)GetSystemService(Context.UsbService);
        //        UsbAccessory[] accessoryList = manager.GetAccessoryList();
        //        if (accessoryList.Any())
        //        {
        //            textView.Text = "accessory attached - Manufacturer: " + accessoryList[0].Manufacturer + " in onnewintent";
        //        }
        //        else
        //        {
        //            textView.Text = "accessory attached - No accessory found in onnewintent";
        //        }
        //    }
        //    else
        //    {
        //        textView.Text = "Accessory not attached - Action was:" + intent.Action + " in onnewintent";
        //    }
        //}

        public void OnAccessoryOpened(FileInputStream stream)
        {
            var tex = FindViewById<TextView>(Resource.Id.textView1);

            tex.Text = "Accessory opened";
        }
    }
}





