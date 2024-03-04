using System;

namespace PasswordSecure.Logic.Implementation;

public class CurrentDateTimeProvider : IDateTimeProvider
{
	public DateTime Now => DateTime.Now;
}
