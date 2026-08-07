using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PasswordSecure.Presentation.Controls.MessageBoxControl;

namespace PasswordSecure.Presentation.Views;

public partial class EditPasswordWindow : Window
{
	public EditPasswordWindow()
	{
		InitializeComponent();

		_isPasswordAccepted = false;
	}

	public int MinimumPasswordLength { get; set; }

	private bool _isPasswordAccepted;

	private string? _initialPassword;

	private void OnLoaded(object? sender, RoutedEventArgs e)
	{
		_initialPassword = TextBoxPassword.Text;

		TextBoxPassword.Focus();
	}

	private void OnClosing(object? sender, WindowClosingEventArgs e)
	{
		if (!_isPasswordAccepted)
		{
			TextBoxPassword.Text = _initialPassword;
		}
	}

	private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
	{
		_isPasswordAccepted = false;

		Close();
	}

	private async void OnOkButtonClick(object? sender, RoutedEventArgs e)
	{
		_isPasswordAccepted = await CanContinue();

		if (_isPasswordAccepted)
		{
			Close();
		}
	}

	private async Task<bool> CanContinue()
	{
		if (IsPasswordTooShort)
		{
			await DisplayPasswordTooShortErrorMessage();

			return false;
		}

		if (IsPasswordMismatch)
		{
			await DisplayPasswordMismatchErrorMessage();

			return false;
		}

		return true;
	}

	private bool IsPasswordTooShort
	{
		get
		{
			var isPasswordTooShort = false;

			if (TextBoxPassword.Text is not null)
			{
				isPasswordTooShort =
					TextBoxPassword.Text.Length < MinimumPasswordLength;
			}

			return isPasswordTooShort;
		}
	}

	private async Task DisplayPasswordTooShortErrorMessage()
	{
		await MessageBoxManager.ShowDialogAsync(
			"Password Too Short Error",
			$"The password is too short.{Environment.NewLine}{Environment.NewLine}Minimum password length is {MinimumPasswordLength} characters.",
			MessageBoxType.Error,
			this);
	}

	private bool IsPasswordMismatch
	{
		get
		{
			var isPasswordMismatch = false;

			if (TextBoxPassword.Text == string.Empty)
			{
				TextBoxPassword.Text = null;
			}

			if (TextBoxConfirmPassword.Text == string.Empty)
			{
				TextBoxConfirmPassword.Text = null;
			}

			if (TextBoxPassword.Text is not null ||
				TextBlockConfirmPassword.Text is not null)
			{
				isPasswordMismatch =
					TextBoxPassword.Text != TextBoxConfirmPassword.Text;
			}

			return isPasswordMismatch;
		}
	}

	private async Task DisplayPasswordMismatchErrorMessage()
	{
		await MessageBoxManager.ShowDialogAsync(
			"Password Mismatch Error",
			"The password and the confirmed password do not match.",
			MessageBoxType.Error,
			this);
	}
}
