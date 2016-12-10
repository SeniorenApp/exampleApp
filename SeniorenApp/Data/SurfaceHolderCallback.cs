using Android.Graphics;
using Android.Runtime;
using Android.Views;
using SeniorenApp.Helper;
using static Android.Hardware.Camera;

namespace SeniorenApp.Data
{
    internal class SurfaceHolderCallback : Java.Lang.Object, ISurfaceHolderCallback
    {
        private Android.Hardware.Camera _Camera;
        private Parameters _Parameters;

        public enum ZoomDirection
        {
            In = 0,
            Out
        };

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(SurfaceChanged), "called.");

            _Camera.StartPreview();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(SurfaceCreated), "called.");

            try
            {
                if (_Camera == null)
                {
                    _Camera = Open();

                    _Parameters = _Camera.GetParameters();

                    if (SupportsFlash())
                    {
                        Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(SurfaceCreated), "flash is supported. Enabling flash.");

                        _Parameters.FlashMode = Parameters.FlashModeTorch;
                    }

                    if (SupportsZoom())
                    {
                        Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(SurfaceCreated), "zoom is supported. Enabling zoom.");

                        _Parameters.Zoom = _Parameters.MaxZoom / 2;
                    }

                    _Camera.SetParameters(_Parameters);
                    _Camera.SetPreviewDisplay(holder);
                }               
            }
            catch (Java.Lang.Exception ex)
            {
                Logger.LogError(ex);
            }            
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(SurfaceDestroyed), "called.");

            _Camera.StopPreview();
            _Camera.Release();
            _Camera.Dispose();
            _Camera = null;
        }

        private bool SupportsFlash()
        {
            return _Parameters == null ? false : _Parameters.SupportedFlashModes.Contains(Parameters.FlashModeTorch);
        }

        private bool SupportsZoom()
        {
            return _Parameters == null ? false : _Parameters.IsZoomSupported;
        }

        public void Zoom(ZoomDirection direction)
        {
            Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(Zoom), "called.");
            Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(Zoom), " direction is: " + direction.ToString());

            if (_Camera != null)
            {
                if (SupportsZoom())
                {
                    Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(Zoom), "zoom is supported.");

                    switch (direction)
                    {
                        case ZoomDirection.In:
                            if (_Parameters.Zoom < _Parameters.MaxZoom)
                            {
                                _Parameters.Zoom++;
                                _Camera.SetParameters(_Parameters);
                            }
                            break;
                        case ZoomDirection.Out:
                            if (_Parameters.Zoom != 0)
                            {
                                _Parameters.Zoom--;
                                _Camera.SetParameters(_Parameters);
                            }
                            break;
                        default:
                            return;
                    }

                    Logger.LogInfo(nameof(SurfaceHolderCallback), nameof(Zoom), "Zoom set to: " + _Parameters.Zoom.ToString());
                }
            }
        }
    }
}