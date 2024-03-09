using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace PasswordSecure.Presentation.Views;

public partial class SetPasswordWindow : Window
{
	public SetPasswordWindow()
	{
		InitializeComponent();

		_isPasswordAccepted = false;
		
		AddHandler(KeyUpEvent, OnKeyPressed, RoutingStrategies.Tunnel);
	}
	
	#region Private
	
	public int MinimumPasswordLength { get; set; }

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
	
	private async void OnKeyPressed(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			_isPasswordAccepted = await CanContinue();

			if (_isPasswordAccepted)
			{
				Close();
			}
			
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
				isPasswordTooShort = TextBoxPassword.Text.Length < MinimumPasswordLength;
			}

			return isPasswordTooShort;
		}
	}
	
	private async Task DisplayPasswordTooShortErrorMessage()
	{
		var passwordTooShortErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
			"Password Too Short Error",
			$"The password is too short.{Environment.NewLine}Minimum password length is {MinimumPasswordLength} characters.",
			ButtonEnum.Ok,
			MsBox.Avalonia.Enums.Icon.Error,
			WindowStartupLocation.CenterOwner);

		await passwordTooShortErrorMessageBox.ShowWindowDialogAsync(this);
	}

	private bool IsPasswordMismatch
		=> TextBoxPassword.Text != TextBoxConfirmPassword.Text;
	
	private async Task DisplayPasswordMismatchErrorMessage()
	{
		var passwordMismatchErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
			"Password Mismatch Error",
			"The password and the confirmed password do not match.",
			ButtonEnum.Ok,
			MsBox.Avalonia.Enums.Icon.Error,
			WindowStartupLocation.CenterOwner);

		await passwordMismatchErrorMessageBox.ShowWindowDialogAsync(this);
	}
	
	#endregion
}
