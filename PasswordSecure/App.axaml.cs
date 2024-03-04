using Avalonia.Markup.Xaml;
using PasswordSecure.Presentation;

namespace PasswordSecure;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IMainView mainView = new MainWindow();
        mainView.Show();
    }
}
