namespace PasswordSecure.Presentation.Views;

public class SetMasterPasswordWindow : SetPasswordWindow
{
	public SetMasterPasswordWindow()
	{
		Title = "Set Master Password";

		_textBlockPassword.Text = "Master password";
		_textBlockConfirmPassword.Text = "Confirm master password";
		_buttonSetPassword.Content = "Set master password";
	}
}
