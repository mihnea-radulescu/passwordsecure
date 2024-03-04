using System;
using PasswordSecure.Application.Helpers;

namespace PasswordSecure.Infrastructure.Helpers;

public class CurrentDateTimeProvider : IDateTimeProvider
{
	public DateTime Now => DateTime.Now;
}
