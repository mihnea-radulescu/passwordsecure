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
        IFileAccessProvider fileAccessProvider = new FileAccessProvider();
        IDateTimeProvider dateTimeProvider = new CurrentDateTimeProvider();
        IAssemblyVersionProvider assemblyVersionProvider = new AssemblyVersionProvider();
        
        IDataSerializationService dataSerializationService = new JsonDataSerializationService();
        IDataEncryptionService dataEncryptionService = new AesDataEncryptionService();
        IBackupService backupService = new BackupService(fileAccessProvider, dateTimeProvider);

        IDataAccessService dataAccessService = new DataAccessService(
            fileAccessProvider,
            dataSerializationService,
            dataEncryptionService,
            backupService);

        var mainWindow = new MainWindow();
        var mainPresenter = new MainPresenter(dataAccessService, assemblyVersionProvider, mainWindow);
        
        mainWindow.Show();
    }
}
