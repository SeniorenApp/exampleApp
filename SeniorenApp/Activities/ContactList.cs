using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using Java.Interop;
using SeniorenApp.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static Android.Widget.AdapterView;

namespace SeniorenApp.Activities
{
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon")]
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

                _Contacts.Clickable = true;
                _Contacts.ItemClick += OnItemClicked;

                _Contacts.Adapter = new ContactListAdapter(this, FindContacts().ToArray());

                _Contacts.ChoiceMode = ChoiceMode.Single;
                _Contacts.SetSelection(0);
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }            
        }  

        private List<string> FindContacts()
        {
            Logger.LogInfo(nameof(ContactList), nameof(FindContacts), " called");

            try
            {
                var contacts = new List<string>();

                ICursor c = ContentResolver.Query(ContactsContract.CommonDataKinds.Phone.ContentUri, null, null, null, null, null);

                while (c.MoveToNext())
                {
                    string name = c.GetString(c.GetColumnIndex(ContactsContract.Contacts.InterfaceConsts.DisplayName));

                    string number = c.GetString(c.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));

                    if (!contacts.Any(x => x.Split(';').First() == name))
                    {
                        contacts.Add(name + ";" + number);
                    }
                }

                return contacts.OrderBy(x => x).ToList();
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
            var child = _Contacts.GetItemAtPosition(((ListView)sender).CheckedItemPosition);

            var number = Android.Net.Uri.Parse(child.ToString().Split(';').Last());

            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " number is: " + number.ToString());

            call.SetData(Android.Net.Uri.Parse("tel:" + number));

            StartActivity(call);

            Logger.LogInfo(nameof(ContactList), nameof(OnItemClicked), " started activity: " + nameof(call));
        }

        [Export("GoToPreviousActivity")]
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
                    if (direction == FocusSearchDirection.Up)
                    {
                        nextItemToSelectPosition = _Contacts.ChildCount - 1;
                    }
                    else if (direction == FocusSearchDirection.Down)
                    {
                        nextItemToSelectPosition = 0;
                    }
                    else if (direction == FocusSearchDirection.Forward)
                    {
                        _GoToPreviousActivity.CallOnClick();
                        return;
                    }

                    _Contacts.SetSelection(nextItemToSelectPosition);
                }
                else
                {
                    nextItemToSelectPosition = GetNextItemToSelect(direction);

                    Logger.LogInfo(nameof(ContactList), nameof(HandleUSBData), "next item to select is: " + nextItemToSelectPosition.ToString());

                    if (direction != FocusSearchDirection.Forward)
                    {
                        if (nextItemToSelectPosition == (_Contacts.ChildCount - 1))
                        {
                            SetFocus(_GoToPreviousActivity);
                            return;
                        }
                        else if (nextItemToSelectPosition == 0)
                        {
                            SetFocus(_GoToPreviousActivity);
                            return;
                        }
                    }
                    
                    _Contacts.SetSelection(nextItemToSelectPosition);

                    if (direction == FocusSearchDirection.Forward)
                    {
                        ((TextView)_Contacts.SelectedItem).CallOnClick();
                    }
                }
                                             
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

            var currentlyFocusedItem = _Contacts.SelectedItemPosition;

            Logger.LogInfo(nameof(ContactList), nameof(GetNextItemToSelect), "currently selected item is: " + currentlyFocusedItem.ToString());

            if (currentlyFocusedItem == -1)
            {
                return 0;
            }
            else if (direction == FocusSearchDirection.Up)
            {
                if (currentlyFocusedItem == 0)
                {
                    return _Contacts.ChildCount - 1;
                }
                else
                {
                    return currentlyFocusedItem - 1;
                }
            }
            else if (direction == FocusSearchDirection.Down)
            {
                if (currentlyFocusedItem == (_Contacts.ChildCount - 1))
                {
                    return 0;
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