using Android.App;
using Android.Content;
using Android.Database;
using Android.Hardware.Usb;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using SeniorenApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Android.Widget.AdapterView;

namespace SeniorenApp
{
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon")]
    public class ContactList : Activity
    {
        ListView _Contacts;
        private static bool _IsActive = false;

        private static bool IsActive
        {
            get
            {
                Logger.LogInfo(nameof(ContactList), nameof(IsActive), "Get called. Value was: " + _IsActive.ToString());

                return _IsActive;
            }
            set
            {
                _IsActive = value;

                Logger.LogInfo(nameof(ContactList), nameof(IsActive), "Set called. Value is now: " + _IsActive.ToString());
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnCreate), " called");
            Logger.LogInfo(nameof(ContactList), nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.ContactsListView);

                _Contacts = FindViewById<ListView>(Resource.Id.ContactsList);
                _Contacts.Clickable = true;
                _Contacts.ItemClick += OnItemClicked;

                ArrayAdapter<string> adapter = new ArrayAdapter<string>(ApplicationContext, Resource.Layout.ContactsListItem, FindContacts());

                _Contacts.Adapter = adapter;
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        protected override void OnNewIntent(Intent intent)
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnNewIntent), " called");
            Logger.LogInfo(nameof(ContactList), nameof(OnNewIntent), " Intent is: " + intent.ToString());

            base.OnNewIntent(intent);

            switch (Intent.Action)
            {
                case UsbManager.ActionUsbAccessoryAttached:
                    Logger.LogInfo(nameof(ContactList), nameof(OnNewIntent), "Accessory attached.");
                    USBHelper.CreateUSBConnection(this, OnUsbDataReceived);
                    break;
            }
        }

        protected override void OnStart()
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnStart), "called.");

            base.OnStart();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnStop()
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnStop), "called.");

            base.OnStop();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.RemoveFromDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = false;
        }

        protected override void OnRestart()
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnRestart), "called.");

            base.OnRestart();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.AddToDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = true;
        }

        protected override void OnDestroy()
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnDestroy), "called.");

            base.OnDestroy();

            if (USBHelper.USBConnection != null)
            {
                USBHelper.USBConnection.RemoveFromDataReceivedEvent(OnUsbDataReceived);
            }

            IsActive = false;
        }

        private List<string> FindContacts()
        {
            Logger.LogInfo(nameof(ContactList), nameof(FindContacts), " called");

            try
            {
                var contacts = new Dictionary<string, string>();

                ICursor c = ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null, null);

                while (c.MoveToNext())
                {
                    string name = c.GetString(c.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));

                    string number = c.GetString(c.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));

                    if (!contacts.ContainsKey(name))
                    {
                        contacts.Add(name, number);
                    }
                }

                return contacts.OrderBy(x => x.Key).Select(x => x.Key + System.Environment.NewLine + x.Value).ToList();
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);

                return new List<string>();
            }      
        }

        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " called");

            var call = new Intent(Intent.ActionCall);
            var child = (TextView)e.View;

            var number = Android.Net.Uri.Parse(child.Text.Split(new[] { System.Environment.NewLine }, StringSplitOptions.None).Last());

            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " number is: " + number.ToString());

            call.SetData(Android.Net.Uri.Parse("tel:" + number));

            StartActivity(call);

            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " started activity: " + nameof(call));
        }

        private void OnUsbDataReceived(byte[] data)
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnUsbDataReceived), "called.");

            if (IsActive)
            {
                Logger.LogInfo(nameof(ContactList), nameof(OnUsbDataReceived), "activity is active.");

                try
                {
                    Logger.LogInfo(nameof(ContactList), nameof(OnUsbDataReceived), data.Length + " bytes received. Message: " + System.BitConverter.ToString(data));

                    USBHelper.InterpretUSBData(data, this, HandleUSBData);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }

        private void HandleUSBData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), nameof(FocusSearchDirection) + " is: " + direction.ToString());

            try
            {
                int nextItemToFocusID = GetNextItemToSelect(direction);

                Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "next item to select is: " + nextItemToFocusID.ToString());

                _Contacts.SetSelection(nextItemToFocusID);

                ((TextView)_Contacts.SelectedItem).CallOnClick();

                Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "click called on: " + _Contacts.SelectedItemId.ToString());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        private int GetNextItemToSelect(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "called.");

            var currentlyFocusedItem = Convert.ToInt32(_Contacts.SelectedItemId);
            int nextItemToFocusID;

            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "currently selected item is: " + currentlyFocusedItem.ToString());

            if (currentlyFocusedItem == -1)
            {
                nextItemToFocusID = 0;
            }
            else if (direction == FocusSearchDirection.Up)
            {
                if (currentlyFocusedItem == 0)
                {
                    nextItemToFocusID = _Contacts.ChildCount - 1;
                }
                else
                {
                    nextItemToFocusID = currentlyFocusedItem - 1;
                }
            }
            else
            {
                if (currentlyFocusedItem == (_Contacts.ChildCount - 1))
                {
                    nextItemToFocusID = 0;
                }
                else
                {
                    nextItemToFocusID = currentlyFocusedItem + 1;
                }
            }

            return nextItemToFocusID;
        }
    }
}