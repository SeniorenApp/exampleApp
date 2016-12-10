using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using SeniorenApp.Data;
using SeniorenApp.Helper;

namespace SeniorenApp.Activities
{
    [Activity(Label = Constants.AboutActivityLabel, MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class About : ActivityBase
    {
        private Button _Back;

        public About()
        {
            _HandleUSBData = HandleUSBData;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Logger.LogInfo(nameof(About), nameof(OnCreate), " called");
            Logger.LogInfo(nameof(About), nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.About);

                var tv = FindViewById<TextView>(Resource.Id.info_text);

                tv.SetText(Constants.AppInfo, TextView.BufferType.Normal);

                Linkify.AddLinks(tv, MatchOptions.All);

                _Back = FindViewById<Button>(Resource.Id.GoBack);
                _Back.RequestFocus();
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        [Export(nameof(GoBack))]
        public void GoBack(View view)
        {
            Logger.LogInfo(nameof(About), nameof(GoBack), " called");

            Finish();
        }

        private void HandleUSBData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(About), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(About), nameof(HandleUSBData), nameof(FocusSearchDirection) + " is: " + direction.ToString());

            try
            {
                if (direction == FocusSearchDirection.Forward)
                {
                    _Back.CallOnClick();
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}