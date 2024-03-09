using System;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class CurrentDateTimeProvider : IDateTimeProvider
{
	public string Now => DateTime.Now.ToString("yyyyMMddHHmmss");
}
