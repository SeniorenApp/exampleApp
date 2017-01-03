using Android.Util;
using SeniorenApp.Data;
using System;

namespace SeniorenApp.Helper
{
    internal static class Logger
    {
        public static void LogInfo(string classname, string functionname, string msg)
        {
            Log.Info(Constants.LoggingLabel, classname + " - " + functionname + " : " + msg);
        }

        public static void LogError(Java.Lang.Exception ex)
        {
            Log.Error(Constants.LoggingLabel, ex.ToString() + Environment.NewLine + Environment.NewLine + ex.GetType().ToString() + Environment.NewLine + ex.Message != null && ex.Message != string.Empty ? ex.Message : "Exception message was empty." + Environment.NewLine + ex.InnerException != null ? ex.InnerException.Message : "InnerException was empty." + Environment.NewLine + ex.StackTrace != null ? ex.StackTrace : "Stacktrace was empty.");
        }

        public static void LogError(Exception ex)
        {
            Log.Error(Constants.LoggingLabel, ex.ToString() + Environment.NewLine + Environment.NewLine + ex.GetType().ToString() + Environment.NewLine + ex.Message != null && ex.Message != string.Empty ? ex.Message : "Exception message was empty." + Environment.NewLine + ex.InnerException != null ? ex.InnerException.Message : "InnerException was empty." + Environment.NewLine + ex.StackTrace != null ? ex.StackTrace : "Stacktrace was empty.");
        }
    }
}