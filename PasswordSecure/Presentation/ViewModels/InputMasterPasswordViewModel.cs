using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure.Presentation.ViewModels;

public class InputMasterPasswordViewModel : ObservableObject, IPasswordViewModel
{
	public InputMasterPasswordViewModel(
		InputMasterPasswordWindow inputMasterPasswordWindow,
		AccessParams accessParams)
	{
		_inputMasterPasswordWindow = inputMasterPasswordWindow;
		_accessParams = accessParams;
	}
	
	public string? Password
	{
		get => _accessParams.Password;
		set
		{
			SetProperty(
				_accessParams.Password,
				value,
				_accessParams,
				(accessParams, propertyValue) => accessParams.Password = propertyValue);
		}
	}
	
	#region Private

	private readonly InputMasterPasswordWindow _inputMasterPasswordWindow;
	private readonly AccessParams _accessParams;
	
	#endregion
}
