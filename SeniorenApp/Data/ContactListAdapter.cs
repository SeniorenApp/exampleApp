using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace SeniorenApp.Data
{
    internal class ContactListAdapter : ArrayAdapter<ContactListItem>
    {
        private Context _Context;
        private List<ContactListItem> _Values;

        public ContactListAdapter(Context context, ContactListItem[] values)
            : base(context, Resource.Layout.ContactsListItem, values)
        {
            _Context = context;
            _Values = values.ToList();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater) _Context.GetSystemService(Context.LayoutInflaterService);

            View row = inflater.Inflate(Resource.Layout.ContactsListItem, parent, false);

            var name = row.FindViewById<TextView>(Resource.Id.Name);
            var number = row.FindViewById<TextView>(Resource.Id.PhoneNumber);

            name.Focusable = true;
            name.FocusableInTouchMode = true;

            name.SetText(_Values[position].Name, TextView.BufferType.Normal);
            number.SetText(_Values[position].Number, TextView.BufferType.Normal);

            return row;
        }
    }
}