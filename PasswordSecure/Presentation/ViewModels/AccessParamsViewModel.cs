using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class AccessParamsViewModel : ObservableObject
{
	public AccessParamsViewModel(AccessParams accessParams)
	{
		_accessParams = accessParams;
	}

	public string? MasterPassword
	{
		get => _accessParams.MasterPassword;
		set
		{
			SetProperty(
				_accessParams.MasterPassword,
				value,
				_accessParams,
				(accessParams, propertyValue) => accessParams.MasterPassword = propertyValue);
		}
	}
	
	#region Private
	
	private readonly AccessParams _accessParams;
	
	#endregion
}
