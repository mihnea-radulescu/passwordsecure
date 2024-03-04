using Avalonia;
using Avalonia.Markup.Xaml;
using PasswordSecure.Controls;

namespace PasswordSecure;

public partial class App : Application
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
