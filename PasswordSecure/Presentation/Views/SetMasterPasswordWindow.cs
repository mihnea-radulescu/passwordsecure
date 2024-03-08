namespace PasswordSecure.Presentation.Views;

public class SetMasterPasswordWindow : SetPasswordWindow
{
	public SetMasterPasswordWindow()
	{
		Title = "Set Master Password";
		TextBlockPassword.Text = "Master password";
		TextBlockConfirmPassword.Text = "Confirm master password";
	}
}
