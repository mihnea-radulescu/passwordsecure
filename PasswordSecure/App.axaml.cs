//#define FLATPAK_BUILD

using Avalonia.Markup.Xaml;
using PasswordSecure.Application.Providers;
using PasswordSecure.Application.Services;
using PasswordSecure.Infrastructure.Providers;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Presentation;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure;

public class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IAssemblyVersionProvider assemblyVersionProvider = new AssemblyVersionProvider();

        IDataAccessService dataAccessService = DataAccessServiceProvider.Create();

        IDataAccessService dataAccessServiceDecorated = new TaskDecoratorDataAccessService(
            dataAccessService
        );

        IEncryptedDataFolderProvider encryptedDataFolderProvider;
#if FLATPAK_BUILD
        encryptedDataFolderProvider = new FlatpakEncryptedDataFolderProvider();
#else
        encryptedDataFolderProvider = new DefaultEncryptedDataFolderProvider();
#endif

        var mainWindow = new MainWindow();
        var mainPresenter = new MainPresenter(
            dataAccessServiceDecorated,
            new AssemblyVersionProvider(),
            encryptedDataFolderProvider,
            mainWindow
        );

        mainWindow.Show();
    }
}
