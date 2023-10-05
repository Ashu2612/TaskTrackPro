using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Windows.Forms;
using Windows.Graphics;
using Application = Microsoft.Maui.Controls.Application;

namespace TaskTrackPro
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent(); Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
#if WINDOWS
                var mauiWindow = handler.VirtualView;
                var nativeWindow = handler.PlatformView;
                nativeWindow.Activate();
                IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                System.Drawing.Rectangle workingRectangle =
                Screen.PrimaryScreen.WorkingArea;
                appWindow.Resize(new SizeInt32(1080, 640));
                // appWindow.MoveAndResize(new Windows.Graphics.RectInt32((Screen.PrimaryScreen.Bounds.Width / 8), (Screen.PrimaryScreen.Bounds.Height / 8), (workingRectangle.Width - (int)((workingRectangle.Width / 100) * 20)), (workingRectangle.Height - (int)((workingRectangle.Height / 100) * 20))));
                var presenter = appWindow.Presenter as OverlappedPresenter;
                presenter.IsResizable = false;
#endif
            });
            MainPage = new AppShell();
        }
    }
}