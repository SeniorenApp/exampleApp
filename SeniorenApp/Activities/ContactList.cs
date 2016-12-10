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
    [Activity(Label = Constants.ContactListActivityLabel, MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class ContactList : ActivityBase
    {
        private ListView _Contacts;
        private Button _GoToPreviousActivity;

        public ContactList()
        {
            _HandleUSBData = HandleUSBData;
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

        private void HandleUSBData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), nameof(FocusSearchDirection) + " is: " + direction.ToString());

            try
            {
                int nextItemToSelectPosition = 0;

                if (_GoToPreviousActivity.IsFocused)
                {
                    Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), nameof(_GoToPreviousActivity) +  " is focused.");

                    if (direction == FocusSearchDirection.Up)
                    {
                        nextItemToSelectPosition = _Contacts.Count - 1;

                        Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "next item to select: " + nextItemToSelectPosition.ToString());
                    }
                    else if (direction == FocusSearchDirection.Down)
                    {
                        nextItemToSelectPosition = 0;
                    }
                    else if (direction == FocusSearchDirection.Forward)
                    {
                        _GoToPreviousActivity.ClearFocus();
                        _GoToPreviousActivity.CallOnClick();
                        return;
                    }

                    _GoToPreviousActivity.ClearFocus();
                    _Contacts.SetItemChecked(nextItemToSelectPosition, true);
                    _Contacts.SetSelection(nextItemToSelectPosition);
                }
                else
                {
                    nextItemToSelectPosition = GetNextItemToSelect(direction);

                    Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "next item to select is: " + nextItemToSelectPosition.ToString());

                    if (direction != FocusSearchDirection.Forward)
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

                    if (direction == FocusSearchDirection.Forward)
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

        private int GetNextItemToSelect(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "called.");

            var currentlyFocusedItem = _Contacts.CheckedItemPosition;

            _Contacts.SetItemChecked(currentlyFocusedItem, false);

            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "currently selected item is: " + currentlyFocusedItem.ToString());

            if (currentlyFocusedItem == -1)
            {
                return 0;
            }
            else if (direction == FocusSearchDirection.Up)
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
            else if (direction == FocusSearchDirection.Down)
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