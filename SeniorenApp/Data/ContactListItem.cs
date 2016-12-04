namespace SeniorenApp.Data
{
    internal class ContactListItem
    {
        public string Name
        {
            get;
            set;
        }

        public string Number
        {
            get;
            set;
        }

        public static explicit operator ContactListItem(Java.Lang.Object obj)
        {
            var propInfo = obj.GetType().GetProperty("Instance");

            return propInfo == null ? null : propInfo.GetValue(obj, null) as ContactListItem;
        }
    }
}