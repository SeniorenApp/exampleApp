using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Text.Util;
using Android.Views;
using Android.Widget;
using Java.Interop;
using Java.IO;
using SeniorenApp.Helper;
using System.Text;

namespace SeniorenApp.Activities
{
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
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
                _Back = FindViewById<Button>(Resource.Id.GoBack);
                _Back.RequestFocus();

                tv.SetText(TranslateHTML(ReadFromTextFile(Resource.Raw.AppInfo)), TextView.BufferType.Normal);

                Linkify.AddLinks(tv, MatchOptions.All);
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }

        [Export("GoBack")]
        public void GoBack(View view)
        {
            Logger.LogInfo(nameof(About), nameof(GoBack), " called");

            Finish();
        }

        private string ReadFromTextFile(int id)
        {
            Logger.LogInfo(nameof(About), nameof(ReadFromTextFile), " called");
            Logger.LogInfo(nameof(About), nameof(ReadFromTextFile), " Id is: " + id.ToString());

            var reader = new InputStreamReader(ApplicationContext.Resources.OpenRawResource(id));
            var bufReader = new BufferedReader(reader);

            var text = new StringBuilder();
            string line;
            
            try
            {
                while ((line = bufReader.ReadLine()) != null)
                {
                    text.Append(line);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);

                return null;
            }

            return text.ToString();
        }
        
        private string TranslateHTML(string text)
        {
            if (((int)Build.VERSION.SdkInt) >= 24)
            {
                return Html.FromHtml(text, FromHtmlOptions.ModeLegacy).ToString();
            }
            else
            {
                return Html.FromHtml(text).ToString();
            }
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