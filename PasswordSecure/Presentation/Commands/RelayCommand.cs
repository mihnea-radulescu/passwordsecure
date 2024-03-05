using System;
using System.Windows.Input;

namespace PasswordSecure.Presentation.Commands;

public class RelayCommand : ICommand
{
	public RelayCommand(
		Action<object?> execute,
		Func<object?, bool>? canExecute = null)
	{
		_execute = execute;
		_canExecute = canExecute;
	}

	public bool CanExecute(object? parameter)
	{
		if (_canExecute is null)
		{
			return true;
		}
		
		return _canExecute(parameter);
	}

	public void Execute(object? parameter) => _execute(parameter);

	public event EventHandler? CanExecuteChanged;
	
	#region Private
	
	private readonly Action<object?> _execute;
	private readonly Func<object?, bool>? _canExecute;
	
	#endregion
}
