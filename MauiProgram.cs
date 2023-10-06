using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Storage.Pickers;

namespace TaskTrackPro
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(wndLifeCycleBuilder =>
                {
                    wndLifeCycleBuilder.OnWindowCreated(window =>
                    {
                        IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                        AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);

                        //hide the minimize and maximize button in title bar
                        window.ExtendsContentIntoTitleBar = true;
                        if (winuiAppWindow.Presenter is OverlappedPresenter p)
                        {
                            p.IsResizable = true;
                            p.IsMaximizable = false;
                            p.SetBorderAndTitleBar(false, false);
                        }
                        winuiAppWindow.Closing += async (s, e) =>
                        {
                            e.Cancel = true;

                            bool result = await App.Current.MainPage.DisplayAlert(
                        "Close the application ?",
                        "Are sure you want to quite ?",
                        "Yes",
                        "No");

                            if (result)
                            {
                                App.Current.Quit();
                            }

                        };
                    });
                });
            });
#endif
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}