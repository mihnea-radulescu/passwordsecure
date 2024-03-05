using Avalonia.Markup.Xaml;
using PasswordSecure.Application.Helpers;
using PasswordSecure.Application.Services;
using PasswordSecure.DomainModel;
using PasswordSecure.Infrastructure.Helpers;
using PasswordSecure.Infrastructure.Services;
using PasswordSecure.Presentation.ViewModels;
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
        var accountEntry1 = new AccountEntry
        {
            Name = "Google",
            Website = "https://mail.google.com",
            User = "john.doe",
            Password = "123456**&&"
        };
		
        var accountEntry2 = new AccountEntry
        {
            Name = "Microsoft",
            Website = "https://azure.microsoft.com",
            User = "john_doe",
            Password = "654321&&**"
        };
        
        IDataSerializationService dataSerializationService = new JsonDataSerializationService();
        IDataEncryptionService dataEncryptionService = new AesDataEncryptionService();
        IFileAccessProvider fileAccessProvider = new FileAccessProvider();

        IDataAccessService dataAccessService = new DataAccessService(
            dataSerializationService,
            dataEncryptionService,
            fileAccessProvider);

        var accountEntries = new AccountEntryCollection { accountEntry1, accountEntry2 };
        var mainWindowViewModel = new AccountEntryCollectionViewModel(dataAccessService, accountEntries);
        
        var mainWindow = new MainWindow
        {
            DataContext = mainWindowViewModel
        };
        
        mainWindow.Show();
    }
}
