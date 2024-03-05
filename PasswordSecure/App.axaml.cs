using Avalonia.Markup.Xaml;
using PasswordSecure.Application.Helpers;
using PasswordSecure.Application.Services;
using PasswordSecure.Infrastructure.Helpers;
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
        IDataSerializationService dataSerializationService = new JsonDataSerializationService();
        IDataEncryptionService dataEncryptionService = new AesDataEncryptionService();
        IFileAccessProvider fileAccessProvider = new FileAccessProvider();

        IDataAccessService dataAccessService = new DataAccessService(
            dataSerializationService,
            dataEncryptionService,
            fileAccessProvider);

        IMainView mainView = new MainWindow();
        var mainPresenter = new MainPresenter(dataAccessService, mainView);
        
        mainView.Show();
    }
}
