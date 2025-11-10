using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.MusicStore.Messages;
using Avalonia.MusicStore.ViewModels;
using Avalonia.MusicStore.Views;
using CommunityToolkit.Mvvm.Messaging;
using System.Linq;
using System.Threading.Tasks;

namespace Avalonia.MusicStore;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            // ✅ Register message to open MusicStoreWindow
            WeakReferenceMessenger.Default.Register<PurchaseAlbumMessage>(this, async (r, m) =>
            {
                var musicStoreWindow = new MusicStoreWindow
                {
                    DataContext = new MusicStoreViewModel()
                };

                // Open it as a dialog (so it’s modal and centered on MainWindow)
                if (desktop.MainWindow != null)
                {
                    await musicStoreWindow.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    musicStoreWindow.Show();
                }

                // We don’t need to return anything here, but we reply to finish the async message
                m.Reply((AlbumViewModel?)null);
            });

            // ✅ Set up your main window
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var toRemove = BindingPlugins.DataValidators
            .OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        foreach (var plugin in toRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}
