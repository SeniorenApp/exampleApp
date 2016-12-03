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

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            _Camera.StartPreview();
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                if (_Camera == null)
                {
                    _Camera = Open();

                    _Parameters = _Camera.GetParameters();

                    if (SupportsFlash())
                    {
                        _Parameters.FlashMode = Parameters.FlashModeTorch;
                    }

                    if (SupportsZoom())
                    {
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

        public void ZoomIn()
        {
            if (_Camera != null)
            {
                if (SupportsZoom())
                {
                    if (_Parameters.Zoom < _Parameters.MaxZoom)
                    {
                        _Parameters.Zoom++;
                        _Camera.SetParameters(_Parameters);
                    }
                }
            }
        }

        public void ZoomOut()
        {
            if (_Camera != null)
            {
                if (SupportsZoom())
                {
                    if (_Parameters.Zoom != 0)
                    {
                        _Parameters.Zoom--;
                        _Camera.SetParameters(_Parameters);
                    }
                }
            }
        }
    }
}