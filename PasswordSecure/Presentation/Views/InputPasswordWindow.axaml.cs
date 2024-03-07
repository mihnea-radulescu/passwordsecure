using Avalonia.Controls;
using Avalonia.Interactivity;

namespace PasswordSecure.Presentation.Views;

public partial class InputPasswordWindow : Window
{
	public InputPasswordWindow()
	{
		InitializeComponent();
	}
	
	private void OnInputPasswordButtonClick(object? sender, RoutedEventArgs e)
		=> Close();
}
