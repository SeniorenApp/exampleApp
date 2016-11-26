using Android.Util;
using System;

namespace SeniorenApp.Helper
{
    internal static class Logger
    {
        public static void LogInfo(string classname, string functionname, string msg)
        {
            Log.Info("Accessory", classname + " - " + functionname + " : " + msg);
        }

        public static void LogError(Exception ex)
        {
            Log.Error("Accessory", ex.ToString() + Environment.NewLine + Environment.NewLine + ex.GetType().ToString() + Environment.NewLine + ex.Message != null ? ex.Message : string.Empty + Environment.NewLine + ex.InnerException != null ? ex.InnerException.Message : string.Empty + Environment.NewLine + ex.StackTrace != null ? ex.StackTrace : string.Empty);
        }
    }
}