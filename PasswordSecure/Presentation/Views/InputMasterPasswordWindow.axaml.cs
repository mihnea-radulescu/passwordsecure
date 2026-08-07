using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PasswordSecure.Presentation.Views;

public partial class InputMasterPasswordWindow : Window
{
	public InputMasterPasswordWindow()
	{
		InitializeComponent();

		_isPasswordAccepted = false;
	}

	private bool _isPasswordAccepted;

	private void OnLoaded(object? sender, RoutedEventArgs e)
		=> TextBoxPassword.Focus();

	private void OnClosing(object? sender, WindowClosingEventArgs e)
	{
		if (!_isPasswordAccepted)
		{
			TextBoxPassword.Text = null;
		}
	}

	private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
	{
		_isPasswordAccepted = false;

		Close();
	}

	private void OnOkButtonClick(object? sender, RoutedEventArgs e)
	{
		_isPasswordAccepted = true;

		Close();
	}
}
