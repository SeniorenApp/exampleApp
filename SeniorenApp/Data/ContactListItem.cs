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

        /// <summary>
        /// Explicit operator for converting the object returned from the view to a ContactListItem.
        /// </summary>
        public static explicit operator ContactListItem(Java.Lang.Object obj)
        {
            var propInfo = obj.GetType().GetProperty("Instance");

            return propInfo == null ? null : propInfo.GetValue(obj, null) as ContactListItem;
        }
    }
}