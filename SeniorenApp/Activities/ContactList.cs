using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Java.Interop;
using SeniorenApp.Data;
using SeniorenApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Android.Widget.AdapterView;

namespace SeniorenApp.Activities
{
    /// <summary>
    /// Contact list activity for selecting an existing contact to call.
    /// For detecting an new intent while this activity is active (USB device plugged in) LaunchMode has been set to SingleTop.
    /// </summary>
    [Activity(Label = Constants.ContactListActivityLabel, MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ContactList : ActivityBase
    {
        private ListView _Contacts;
        private Button _GoToPreviousActivity;

        public ContactList()
        {
            _HandleUSBData = HandleUSBData;
            _OnConnectionClosed = OnUSBConnectionClosed;
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
                _GoToPreviousActivity = FindViewById<Button>(Resource.Id.GoBack);

                EnableFocusable(_GoToPreviousActivity);

                _Contacts.Adapter = new ContactListAdapter(this, FindContacts().ToArray());

                _Contacts.Clickable = true;
                _Contacts.ChoiceMode = ChoiceMode.Single;
                _Contacts.ItemClick += OnItemClicked;

                _Contacts.SetItemChecked(0, true);
                _Contacts.SetSelection(0);
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            EnableFocusable(_GoToPreviousActivity);
        }

        /// <summary>
        /// Finds all existing contacts querieng the phone database.
        /// </summary>
        /// <returns></returns>
        private List<ContactListItem> FindContacts()
        {
            Logger.LogInfo(nameof(ContactList), nameof(FindContacts), " called");

            try
            {
                var contacts = new List<ContactListItem>();

                ICursor c = ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null, null);

                while (c.MoveToNext())
                {
                    string name = c.GetString(c.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));
                    string number = c.GetString(c.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));

                    if (!contacts.Any(x => x.Name.Trim().ToLower() == name.Trim().ToLower()))
                    {
                        contacts.Add(new ContactListItem() { Name = name, Number = number });
                    }
                }

                return contacts.OrderBy(x => x.Name).ToList();
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);

                return new List<ContactListItem>();
            }      
        }

        private void OnItemClicked(object sender, ItemClickEventArgs e)
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " called");

            var call = new Intent(Intent.ActionCall);
            var selectedItem = (ContactListItem)_Contacts.GetItemAtPosition(((ListView)sender).CheckedItemPosition);

            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " number is: " + selectedItem.Number);

            call.SetData(Android.Net.Uri.Parse("tel:" + selectedItem.Number));

            StartActivity(call);

            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " started activity: " + nameof(call));
        }

        [Export(nameof(GoToPreviousActivity))]
        public void GoToPreviousActivity(View view)
        {
            Logger.LogInfo(nameof(ContactList), nameof(GoToPreviousActivity), "called.");

            Finish();
        }

        private void OnUSBConnectionClosed()
        {
            Logger.LogInfo(nameof(ContactList), nameof(OnUSBConnectionClosed), " called");

            DisableFocusable(_GoToPreviousActivity);
        }

        private void HandleUSBData(USBCommand command)
        {
            Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), nameof(command) + " is: " + command.ToString());

            try
            {
                int nextItemToSelectPosition = 0;

                // If goback button is focused...
                if (_GoToPreviousActivity.IsFocused)
                {
                    Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), nameof(_GoToPreviousActivity) +  " is focused.");

                    // if command is up set itemposition to last item in list...
                    if (command == USBCommand.up)
                    {
                        nextItemToSelectPosition = _Contacts.Count - 1;

                        Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "next item to select: " + nextItemToSelectPosition.ToString());
                    }
                    // if command is down set itemposition to first item in list...
                    else if (command == USBCommand.down)
                    {
                        nextItemToSelectPosition = 0;
                    }
                    // if command is ok call goback function.
                    else if (command == USBCommand.ok)
                    {
                        _GoToPreviousActivity.ClearFocus();
                        _GoToPreviousActivity.CallOnClick();
                        return;
                    }

                    // Set new item position.
                    _GoToPreviousActivity.ClearFocus();
                    _Contacts.SetItemChecked(nextItemToSelectPosition, true);
                    _Contacts.SetSelection(nextItemToSelectPosition);
                }
                else
                {
                    // If goback button is not focused. Get the next item to select calling getnextitemtoselect.
                    nextItemToSelectPosition = GetNextItemToSelect(command);

                    Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "next item to select is: " + nextItemToSelectPosition.ToString());

                    if (command != USBCommand.ok)
                    {
                        if (nextItemToSelectPosition == -1)
                        {
                            _GoToPreviousActivity.RequestFocus();
                            return;
                        }
                    }

                    Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "set item checked: " + nextItemToSelectPosition.ToString());

                    _Contacts.SetItemChecked(nextItemToSelectPosition, true);
                    _Contacts.SetSelection(nextItemToSelectPosition);

                    if (command == USBCommand.ok)
                    {
                        ((TextView)_Contacts.SelectedItem).CallOnClick();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Pretty complicated functions that returns the new item to select depending the given command.
        /// </summary>
        private int GetNextItemToSelect(USBCommand command)
        {
            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "called.");

            var currentlyFocusedItem = _Contacts.CheckedItemPosition;

            _Contacts.SetItemChecked(currentlyFocusedItem, false);

            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "currently selected item is: " + currentlyFocusedItem.ToString());

            if (currentlyFocusedItem == -1)
            {
                return 0;
            }
            else if (command == USBCommand.up)
            {
                if (currentlyFocusedItem == 0)
                {
                    return -1;
                }
                else
                {
                    return currentlyFocusedItem - 1;
                }
            }
            else if (command == USBCommand.down)
            {
                if (currentlyFocusedItem == (_Contacts.Count - 1))
                {
                    return -1;
                }
                else
                {
                    return currentlyFocusedItem + 1;
                }
            }     
            else
            {
                return currentlyFocusedItem;
            }                               
        }
    }
}