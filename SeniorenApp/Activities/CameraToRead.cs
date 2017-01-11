using Android.App;
using Android.OS;
using Android.Views;
using SeniorenApp.Data;
using SeniorenApp.Helper;

namespace SeniorenApp.Activities
{
    /// <summary>
    /// Camera activity for reading with zoom and flashlight.
    /// For detecting an new intent while this activity is active (USB device plugged in) LaunchMode has been set to SingleTop.
    /// </summary>
    [Activity(Label = "Accessory", MainLauncher = false, Icon = "@drawable/icon", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
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

        private void HandleUSBData(USBCommand command)
        {
            Logger.LogInfo(nameof(CameraToRead), nameof(HandleUSBData), "called.");
            Logger.LogInfo(nameof(CameraToRead), nameof(HandleUSBData), nameof(command) + " is: " + command.ToString());

            try
            {
                if (command == USBCommand.ok)
                {
                    Finish();
                }
                else if (command == USBCommand.up)
                {
                    _CameraHandler.Zoom(ZoomDirection.In);
                }
                else if (command == USBCommand.down)
                {
                    _CameraHandler.Zoom(ZoomDirection.Out);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}