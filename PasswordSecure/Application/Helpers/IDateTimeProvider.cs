using System;

namespace PasswordSecure.Application.Helpers;

public interface IDateTimeProvider
{
	public DateTime Now { get; }
}
