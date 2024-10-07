using System;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class CurrentDateTimeProvider : IDateTimeProvider
{
	public string Now => DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
}
