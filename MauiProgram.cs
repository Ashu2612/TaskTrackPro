using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using SkiaSharp.Views.Maui.Controls.Hosting;
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
                .UseMicrocharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FA-Free-Regular-400.0tf", "FontAwesome");
                    fonts.AddFont("FA-Regular-400.0tf", "FABR");
                    fonts.AddFont("FA-Free-Solid-900.0tf", "FAS");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
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
                            p.IsResizable = false;
                            p.IsMaximizable = false;
                            p.IsMinimizable = false;
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