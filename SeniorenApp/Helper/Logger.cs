using Android.Util;
using System;

namespace SeniorenApp.Helper
{
    internal static class Logger
    {
        public static void LogInfo(string location, string msg)
        {
            Log.Info("Accessory", location + " : " + msg);
        }

        public static void LogError(Exception ex)
        {
            Log.Error("Accessory", ex.ToString() + System.Environment.NewLine + ex.Message + System.Environment.NewLine + ex.InnerException != null ? ex.InnerException.Message : string.Empty + System.Environment.NewLine + ex.StackTrace);
        }
    }
}