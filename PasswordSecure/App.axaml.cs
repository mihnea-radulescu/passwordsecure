using System.Collections.Generic;
using Avalonia.Markup.Xaml;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation;
using PasswordSecure.Presentation.ViewModels;

namespace PasswordSecure;

public partial class App : Avalonia.Application
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

        var accountEntries = new List<AccountEntry> { accountEntry1, accountEntry2 };
        var mainWindowViewModel = new AccountEntryCollectionViewModel(accountEntries);
        
        var mainWindow = new MainWindow
        {
            DataContext = mainWindowViewModel
        };
        
        mainWindow.Show();
    }
}
