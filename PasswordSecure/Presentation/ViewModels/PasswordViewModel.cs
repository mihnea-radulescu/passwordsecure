using CommunityToolkit.Mvvm.ComponentModel;
using PasswordSecure.DomainModel;

namespace PasswordSecure.Presentation.ViewModels;

public class PasswordViewModel : ObservableObject
{
	public PasswordViewModel(IPasswordContainer passwordContainer)
	{
		_passwordContainer = passwordContainer;
	}

	public string? Password
	{
		get => _passwordContainer.Password;
		set
		{
			SetProperty(
				_passwordContainer.Password,
				value,
				_passwordContainer,
				(passwordContainer, propertyValue) => passwordContainer.Password = propertyValue);
		}
	}
	
	#region Private
	
	private readonly IPasswordContainer _passwordContainer;
	
	#endregion
}
