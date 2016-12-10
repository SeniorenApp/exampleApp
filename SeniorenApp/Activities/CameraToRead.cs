using Android.App;
using Android.OS;
using Android.Views;
using SeniorenApp.Data;
using SeniorenApp.Helper;

namespace SeniorenApp.Activities
{
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    [UsesLibrary("android.hardware.camera")]
    public class CameraToRead : ActivityBase
    {
        private SurfaceHolderCallback _CameraHandler;

        public CameraToRead()
        {
            _HandleUSBData = HandleUSBData;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Logger.LogInfo(nameof(CameraToRead), nameof(OnCreate), " called");
            Logger.LogInfo(nameof(CameraToRead), nameof(OnCreate), " Intent is: " + Intent.ToString());

            try
            {
                base.OnCreate(savedInstanceState);

                RequestWindowFeature(WindowFeatures.NoTitle);

                Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

                RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

                SetContentView(Resource.Layout.Camera);

                var surface = FindViewById <SurfaceView>(Resource.Id.Preview);

                surface.Holder.SetType(SurfaceType.PushBuffers);

                _CameraHandler = new SurfaceHolderCallback();

                surface.Holder.AddCallback(_CameraHandler);
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        private void HandleUSBData(FocusSearchDirection direction)
        {
            Logger.LogInfo(nameof(CameraToRead), nameof(HandleUSBData), "called.");

            try
            {
                if (direction == FocusSearchDirection.Forward)
                {
                    Finish();
                }
                else if (direction == FocusSearchDirection.Up)
                {
                    _CameraHandler.Zoom(SurfaceHolderCallback.ZoomDirection.In);
                }
                else if (direction == FocusSearchDirection.Down)
                {
                    _CameraHandler.Zoom(SurfaceHolderCallback.ZoomDirection.Out);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}