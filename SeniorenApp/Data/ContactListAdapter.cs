using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace SeniorenApp.Activities
{
    internal class ContactListAdapter : ArrayAdapter<string>
    {
        private Context _Context;
        private List<string> _Values;

        public ContactListAdapter(Context context, string[] values)
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

            name.SetText(_Values[position].Split(';').First(), TextView.BufferType.Normal);
            number.SetText(_Values[position].Split(';').Last(), TextView.BufferType.Normal);

            return row;
        }
    }
}