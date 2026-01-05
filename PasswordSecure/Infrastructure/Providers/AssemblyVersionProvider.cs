using System;
using System.Reflection;
using PasswordSecure.Application.Providers;

namespace PasswordSecure.Infrastructure.Providers;

public class AssemblyVersionProvider : IAssemblyVersionProvider
{
	public AssemblyVersionProvider()
	{
		var major = AssemblyVersion.Major;
		var minor = AssemblyVersion.Minor;
		var build = AssemblyVersion.Build.ToString().PadLeft(2, '0');
		var revision = AssemblyVersion.Revision.ToString().PadLeft(2, '0');

		AssemblyVersionString = $"Version {major}.{minor}.{build}.{revision}";
	}

	public string AssemblyVersionString { get; }

	private static readonly Version AssemblyVersion = Assembly.GetEntryAssembly()!.GetName().Version!;
}
