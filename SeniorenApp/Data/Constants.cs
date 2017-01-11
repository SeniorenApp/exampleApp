using Android.Views;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SeniorenApp.Data
{
    internal static class Constants
    {
        // Constant appinfo string displayed in the appinfo activity.
        // Contains the current version and gives credit to the icon creators.
        public static readonly string AppInfo = new StringBuilder()
                .Append(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title)
                .AppendLine()
                .AppendLine()
                .AppendLine(Assembly.GetExecutingAssembly().GetName().Version.ToString())
                .AppendLine()
                .AppendLine(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright)
                .AppendLine()
                .AppendLine("Icons made by Zurb (http://www.zurb.com) from http://www.flaticon.com")
                .ToString();

        // The commandmapper maps the current usbcommand to the focussearchdirection enum used for selecting the next
        // item to focus.
        public static readonly Dictionary<USBCommand, FocusSearchDirection> CommandMap = new Dictionary<USBCommand, FocusSearchDirection>()
        {
            { USBCommand.up, FocusSearchDirection.Up },
            { USBCommand.down, FocusSearchDirection.Down },
            { USBCommand.left, FocusSearchDirection.Left },
            { USBCommand.right, FocusSearchDirection.Right },
        };

        // Byte used to detect the end of transmission in the usb connection class.
        // 0x04 is the EoT (End of Transmission) byte according to ANSII-Standard.
        public const byte EndOfStreamByte = 0x04;

        // Max size per USB package. Arbitrarily chosen.
        public const int USBMaxPacketSize = 1024;

        // General constants used in different activities.
        public const string AccessoryFilterLocation = "@xml/accessory_filter";
        public const string AndroidUSBHostLibrary = "android.hardware.usb.host";

        public const string MainActivityLabel = "Senioren App";
        public const string AboutActivityLabel = "About the App";
        public const string ContactListActivityLabel = "Your contact list";
        public const string ManualPhoneCallActivityLabel = "Make a call";

        // Label used for logging. All Log-Entries will use this label.
        public const string LoggingLabel = "Accessory";
    }
}