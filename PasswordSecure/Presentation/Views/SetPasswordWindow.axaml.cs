using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PasswordSecure.Presentation.Views;

public partial class SetPasswordWindow : Window
{
	public SetPasswordWindow()
	{
		InitializeComponent();
	}

	private void OnSetPasswordButtonClick(object? sender, RoutedEventArgs e)
		=> Close();
}
