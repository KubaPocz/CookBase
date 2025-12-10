using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace CookBase
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

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(window =>
                {
                    window.OnWindowCreated(w =>
                    {
                        w.ExtendsContentIntoTitleBar = true;
                    });
                });
#endif
            });

            return builder.Build();
        }
    }
}
