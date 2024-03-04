using System;

namespace PasswordSecure.Logic;

public interface IDateTimeProvider
{
	public DateTime Now { get; }
}
