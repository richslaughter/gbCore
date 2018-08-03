using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using GbCore.Ui.ViewModels;
using GbCore.Ui.Views;

namespace GbCore.Ui
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}