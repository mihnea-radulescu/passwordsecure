namespace PasswordSecure.Presentation.Views;

public class CreateMasterPasswordWindow : EditPasswordWindow
{
	public CreateMasterPasswordWindow()
	{
		Title = "Create Master Password";
		TextBlockPassword.Text = "Master password";
		TextBlockConfirmPassword.Text = "Confirm master password";
	}
}
