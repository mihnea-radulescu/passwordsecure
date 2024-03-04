using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PasswordSecure.Presentation.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	#region Protected
	
	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
	
	#endregion
}
