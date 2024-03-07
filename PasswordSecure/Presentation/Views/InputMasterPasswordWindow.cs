namespace PasswordSecure.Presentation.Views;

public class InputMasterPasswordWindow : InputPasswordWindow
{
	public InputMasterPasswordWindow()
	{
		Title = "Input Master Password";

		_textBlockPassword.Text = "Master password";
		_buttonInputPassword.Content = "Input master password";
	}
}
