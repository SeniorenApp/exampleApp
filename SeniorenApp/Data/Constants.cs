using System.Reflection;
using System.Text;

namespace SeniorenApp.Data
{
    internal static class Constants
    {
        public static readonly string AppInfo = new StringBuilder()
                .Append(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title)
                .AppendLine()
                .AppendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString())
                .AppendLine()
                .AppendLine(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright)
                .AppendLine()
                .AppendLine("Icons made by Zurb (http://www.zurb.com) from http://www.flaticon.com")
                .ToString();

        public const byte EndOfStreamByte = 0x04;
        public const int USBMaxPacketSize = 2524;

        public const string AccessoryFilterLocation = "@xml/accessory_filter";
        public const string AndroidUSBHostLibrary = "android.hardware.usb.host";

        public const string MainActivityLabel = "Senioren App";
        public const string AboutActivityLabel = "About the App";
        public const string ContactListActivityLabel = "Your contact list";
        public const string ManualPhoneCallActivityLabel = "Make a call";
    }
}