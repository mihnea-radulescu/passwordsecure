using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class InputMasterPasswordViewModel : ObservableObject, IPasswordViewModel
{
	public InputMasterPasswordViewModel(AccessParams accessParams)
	{
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

	private readonly AccessParams _accessParams;

	#endregion
}
