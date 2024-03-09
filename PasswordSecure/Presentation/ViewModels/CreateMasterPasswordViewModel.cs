using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;
using PasswordSecure.Presentation.Views;

namespace PasswordSecure.Presentation.ViewModels;

public class CreateMasterPasswordViewModel : ObservableObject, IPasswordViewModel
{
	public CreateMasterPasswordViewModel(
		CreateMasterPasswordWindow createMasterPasswordWindow,
		AccessParams accessParams)
	{
		_createMasterPasswordWindow = createMasterPasswordWindow;
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

	private readonly CreateMasterPasswordWindow _createMasterPasswordWindow;
	private readonly AccessParams _accessParams;
	
	#endregion
}
