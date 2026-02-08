using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class CreateMasterPasswordViewModel
	: ObservableObject, IPasswordViewModel
{
	public CreateMasterPasswordViewModel(AccessParams accessParams)
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
				(accessParams, propertyValue)
					=> accessParams.Password = propertyValue);
		}
	}

	private readonly AccessParams _accessParams;
}
