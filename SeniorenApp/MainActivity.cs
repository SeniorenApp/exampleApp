using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Widget;
using Java.IO;
using System;

namespace SeniorenApp
{

    [Activity(Label = "SeniorenApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView textView;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);

                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.Main);

                textView = FindViewById<TextView>(Resource.Id.textView1);
                textView.Text = "bla";

                //var i = new IntentFilter();
                //i.AddAction(UsbManager.ActionUsbAccessoryAttached);
                //i.AddAction(UsbManager.ActionUsbAccessoryDetached);
                //i.AddAction("USBPERMISSION");

                UsbAccessory usb = (UsbAccessory)Intent.GetParcelableExtra(UsbManager.ExtraAccessory);

                if (usb != null)
                {
                    textView.Text = "device found";
                }
                else
                {
                    textView.Text = "device not found";
                }

                if (UsbManager.ActionUsbAccessoryAttached.Equals(Intent.Action))
                {
                    usb = (UsbAccessory)Intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    textView.Text = "accessory attached" + usb != null ? " - Could not create Accessory" : " - Accessory created";
                }
                else
                {
                    textView.Text = "Accessory not attached - Action was:" + Intent.Action;
                }
            }
            catch (Exception ex)
            {
                textView.Text = ex.ToString() + System.Environment.NewLine + ex.GetType().ToString() + System.Environment.NewLine + ex.InnerException.ToString();
            }
            

            // RegisterReceiver(new Receiver((UsbManager)GetSystemService(UsbService), OnAccessoryOpened), i);
        }

        protected override void OnNewIntent(Intent intent)
        {
            try
            {
                UsbAccessory usb = (UsbAccessory)intent.GetParcelableExtra(UsbManager.ExtraAccessory);

                if (usb != null)
                {
                    textView.Text = "device found in onnewintent";
                }
                else
                {
                    textView.Text = "device not found in onnewintent";
                }

                if (UsbManager.ActionUsbAccessoryAttached.Equals(Intent.Action))
                {
                    usb = (UsbAccessory)Intent.GetParcelableExtra(UsbManager.ExtraAccessory);
                    textView.Text = "accessory attached" + usb != null ? " - Could not create Accessory" : " - Accessory created in onnewintent";
                }
                else
                {
                    textView.Text = "Accessory not attached - Action was:" + Intent.Action + " in onnewintent";
                }
            }
            catch (Exception ex)
            {
                textView.Text = ex.ToString() + System.Environment.NewLine + ex.GetType().ToString() + System.Environment.NewLine + ex.InnerException.ToString();
            }          
        }

        public void OnAccessoryOpened(FileInputStream stream)
        {
            var tex = FindViewById<TextView>(Resource.Id.textView1);

            tex.Text = "Accessory opened";
        }
    }
}

    



