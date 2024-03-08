using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace PasswordSecure.Presentation.Views;

public partial class SetPasswordWindow : Window
{
	public SetPasswordWindow()
	{
		InitializeComponent();
		
		AddHandler(KeyUpEvent, OnKeyPressed, RoutingStrategies.Tunnel);
	}
	
	#region Private

	private void OnLoaded(object? sender, RoutedEventArgs e)
		=> TextBoxPassword.Focus();

	private void OnKeyPressed(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			Close();

			e.Handled = true;
		}
	}
	
	private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
		=> Close();
	
	private void OnOkButtonClick(object? sender, RoutedEventArgs e)
		=> Close();
	
	#endregion
}
