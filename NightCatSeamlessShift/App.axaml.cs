using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace NightCatSeamlessShift;

public partial class App : Application
{
    public static IStorageProvider? StorageProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;
            StorageProvider = mainWindow.StorageProvider;
        }

        base.OnFrameworkInitializationCompleted();
    }
}