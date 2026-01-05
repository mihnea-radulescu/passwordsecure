using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace PasswordSecure.Presentation.Views;

public partial class InputMasterPasswordWindow : Window
{
	public InputMasterPasswordWindow()
	{
		InitializeComponent();

		_isPasswordAccepted = false;

		AddHandler(KeyUpEvent, OnKeyPressed, RoutingStrategies.Tunnel);
	}

	private bool _isPasswordAccepted;

	private void OnLoaded(object? sender, RoutedEventArgs e) => TextBoxPassword.Focus();

	private void OnClosing(object? sender, WindowClosingEventArgs e)
	{
		if (!_isPasswordAccepted)
		{
			TextBoxPassword.Text = null;
		}
	}

	private void OnKeyPressed(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			_isPasswordAccepted = true;

			Close();

			e.Handled = true;
		}
		else if (e.Key == Key.Escape)
		{
			_isPasswordAccepted = false;

			Close();

			e.Handled = true;
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
